using CloudPic.BAL.Interfaces;
using CloudPic.BAL.Patterns.Factory;
using CloudPic.BAL.Prototype;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using CloudPic.Models.Enums;
using CloudPic.Models.VM;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostSharp.Patterns.Caching;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class PhotoService : IPhotoService
    {
        private readonly ILogger<PhotoService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPhotoRepo _photoRepo;
        private readonly IPlanRepo _planRepo;
        private readonly IFSRepo _fsRepo;

        public PhotoService(ILogger<PhotoService> logger,
            IConfiguration configuration,
            IPhotoRepo photoRepo,
            IPlanRepo planRepo)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._photoRepo = photoRepo;
            this._planRepo = planRepo;
            this._fsRepo = FileShareFactory.GetFSRepo(FSType.AzureFS, configuration);
        }


        [Cache]
        public async Task<IEnumerable<PhotoHashtagVM>> GetPhotosDetailedForUser(int userId)
        {
            var photos = await GetPhotosForUser(userId);
            var photosWithHashtags = new List<PhotoHashtagVM>();
            foreach (var photo in photos)
            {
                var hashtags = await _photoRepo.GetHashtagsForPhoto(photo.Id);
                photosWithHashtags.Add(new PhotoHashtagVM(photo, hashtags.ToList()));
                photosWithHashtags = photosWithHashtags.OrderByDescending(x => x.UploadDate).ToList();
            }
            return photosWithHashtags;
        }

        [InvalidateCache(nameof(GetPhotosDetailedForUser))]
        public async Task<int> DeletePhoto(int userId)
        {
            await _photoRepo.DeleteHashtagsForPhotoAsync(userId);
            return await _photoRepo.DeletePhotoAsync(userId);
        }

        [InvalidateCache(nameof(GetPhotosDetailedForUser))]
        public async Task<(int, int)> UploadPhoto(PhotoInsertVM photoInsertVM, int userId)
        {
            var guid = Guid.NewGuid().ToString();

            // Adding Photo metadata to database
            double sizeInMb = ConvertBytesToMegabytes(photoInsertVM.File.Length);

            var canUserInserPhoto = await CanUserInsertPhoto(userId, sizeInMb);
            if (canUserInserPhoto < 1)
                return (canUserInserPhoto, 0);

            var photo = new Photo(0, guid, userId, DateTime.Now, photoInsertVM.Description, photoInsertVM.Format, sizeInMb);
            var photoId = int.Parse((await _photoRepo.InsertPhotoAsync(photo)).ToString());

            // Converting image
            var imageFormat = ParseImageFormat(photoInsertVM.Format);

            using (MemoryStream outStreamImage = new MemoryStream())
            {
                // Original image
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    var resizeLayer = new ResizeLayer(new Size(photoInsertVM.Width, photoInsertVM.Height), ResizeMode.Stretch);

                    imageFactory.Load(photoInsertVM.File.OpenReadStream())
                        .Resize(resizeLayer)
                        .Format(imageFormat)
                        .Save(outStreamImage);
                }

                var fileVM = new FileVM(outStreamImage.ToArray(), photoInsertVM.File.ContentType, guid, photoInsertVM.Format);
                _ = await _fsRepo.PostFileAsync(fileVM);

                //  Thumbnail
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    var resizeLayer = new ResizeLayer(new Size(366, 238), ResizeMode.Min);

                    imageFactory.Load(photoInsertVM.File.OpenReadStream())
                        .Resize(resizeLayer)
                        .Format(imageFormat)
                        .Save(outStreamImage);
                }

                var thumbnailVM = new FileVM(outStreamImage.ToArray(), photoInsertVM.File.ContentType, "thumbnails/" + guid, photoInsertVM.Format);
                _ = await _fsRepo.PostFileAsync(thumbnailVM);
            }

            // Handling hashtags
            await InsertTags(photoInsertVM.Tags, photoId);
            return (1, photoId);
        }


        public async Task<int> UpdatePhoto(PhotoUpdateVM photoUpdateVM)
        {
            await _photoRepo.DeleteHashtagsForPhotoAsync(photoUpdateVM.Id);
            await InsertTags(photoUpdateVM.Tags, photoUpdateVM.Id);

            return await _photoRepo.UpdatePhotoAsync(photoUpdateVM.Id, photoUpdateVM.Description);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _photoRepo.GetPhotoAsync(id);
            return photo;
        }

        private async Task<int> CanUserInsertPhoto(int userId, double sizeInMb)
        {
            var lastPackageForUser = await _planRepo.GetLastPackage(userId);
            var plan = await _planRepo.GetPlan(lastPackageForUser.PlanId);
            var consumation = await _planRepo.GetConsumeOnDate(userId, DateTime.Today);


            if (consumation.SizeInMb + sizeInMb > plan.MbPerDay)
                return -1;
            if (consumation.NumberOfPhotos + 1 > plan.PhotosPerDay)
                return -2;
            if (consumation.Storage + sizeInMb > plan.StorageSize)
                return -3;

            return 1;
        }

        public async Task<IEnumerable<Photo>> GetPhotosForUser(int userId)
        {
            return await _photoRepo.GetPhotosForUserAsync(userId);
        }


        private async Task InsertTags(string tags, int photoId)
        {
            if (tags.Length > 0)
                if (tags.Contains(','))
                    tags
                        .Split(',')
                        .Select(t => new Hashtag(t.Trim(), photoId))
                        .Where(t => t.Name.Length > 0)
                        .ToList()
                        .ForEach(async t => await _photoRepo.InsertHashTagAsync(t));
                else
                    if (tags.Trim().Length > 0)
                    await _photoRepo.InsertHashTagAsync(new Hashtag(tags.Trim(), photoId));
        }

        [Cache]
        public static FormatBase ParseImageFormat(string formatToParse)
        {
            return (formatToParse.ToLower()) switch
            {
                "jpg" => new JpegFormat { Quality = 100, IsIndexed = false },
                "jpeg" => new JpegFormat { Quality = 100, IsIndexed = false },
                "png" => new PngFormat { Quality = 100, IsIndexed = false },
                "bmp" => new BitmapFormat { Quality = 100, IsIndexed = false },
                "gif" => new GifFormat { Quality = 100, IsIndexed = false },
                _ => new PngFormat { Quality = 100, IsIndexed = false },
            };
        }

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (double)((bytes / 1024f) / 1024f);
        }

        public byte[] DownloadPhoto(byte[] data, PhotoDownloadVM photoDownload)
        {
            var imageFormat = ParseImageFormat(photoDownload.Format);

            var originalImage = new BAL.Prototype.Image(data);
            var blurredImage = originalImage.Clone().GaussianBlur((int)photoDownload.GaussianBlurIntensity);
            var sharpenedImage = blurredImage.Clone().GaussianSharpen((int)photoDownload.GaussianSharpenIntensity);
            var saturationImage = sharpenedImage.Clone().Saturation((int)photoDownload.SaturationIntensity);

            using (MemoryStream outStreamImage = new MemoryStream())
            {
                // Original image
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    if (photoDownload.Width != 0 && photoDownload.Height != 0)
                    {
                        var resizeLayer = new ResizeLayer(new Size(photoDownload.Width, photoDownload.Height), ResizeMode.Stretch);
                        imageFactory.Load(saturationImage.Data)
                                    .Resize(resizeLayer)
                                    .Format(imageFormat)
                                    .Save(outStreamImage);
                    }
                    else
                    {
                        imageFactory.Load(saturationImage.Data)
                                    .Format(imageFormat)
                                    .Save(outStreamImage);
                    }
                }
                return outStreamImage.ToArray();
            }
        }
    }
}
