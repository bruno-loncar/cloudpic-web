using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudPic.Models.VM;
using System.Threading.Tasks;
using CloudPic.DAL.Interfaces;
using System.Linq;
using System.Collections.Generic;
using CloudPic.Models.DTO;
using CloudPic.BAL.Interfaces;
using CloudPic.BAL.Logger;
using static CloudPic.BAL.Logger.Logger;
using System;
using CloudPic.BAL.CriteriaFilter;

namespace CloudPic.Web.Controllers
{
    public class HomeController : _BaseController
    {

        #region Fields

        private readonly string ERROR_MESSAGE = "Error while sending a request to home service.";

        private readonly IPhotoService _photoService;
        private readonly IPhotoRepo _photoRepo;
        private readonly IUserRepo _userRepo;
        private readonly Logger _logger;

        #endregion

        #region Constructors

        public HomeController(
            IPhotoService photoService,
            IPhotoRepo photoRepo,
            IUserRepo userRepo)
        {
            _photoService = photoService;
            _photoRepo = photoRepo;
            _userRepo = userRepo;

            this._logger = new ConsoleLogger(LogSeverity.Info,
                new ConsoleLogger(LogSeverity.Warning,
                new ConsoleLogger(LogSeverity.Error,
                new FileLogger(LogSeverity.Error))));
        }

        #endregion

        #region Endpoints

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        [Route("explore")]
        public async Task<IActionResult> Explore([FromQuery] SearchParamsVM searchParams)
        {
            try
            {
                var photos = new List<Photo>();
                var explorePhotos = new List<ExplorePhotoVM>();

                if (searchParams.IsInitialized())
                {
                    var allPhotos = (await _photoRepo.GetAllPhotos())
                        .OrderByDescending(x => x.UploadDate).ToList();

                    if (searchParams.Tags != default)
                    {
                        var tags = new List<string>();
                        if (searchParams.Tags.Contains(','))
                            searchParams.Tags
                                .Split(',')
                                .Select(t => new Hashtag(t.Trim(), 0))
                                .Where(t => t.Name.Length > 0)
                                .ToList()
                                .ForEach(t => tags.Add(t.Name));
                        else
                            tags.Add(searchParams.Tags);

                        var photoSet = new HashSet<Photo>();

                        foreach (var tag in tags)
                        {
                            (await _photoRepo.GetPhotosHashtag(tag))
                                .ToList().ForEach(x => photoSet.Add(x));
                        }

                        allPhotos = photoSet.ToList();
                    }


                    ICriteria criteria;

                    criteria = new AuthorCriteria(searchParams.AuthorId);
                    allPhotos = criteria.MeetCriteria(allPhotos);

                    criteria = new DateFromCriteria(searchParams.DateFrom);
                    allPhotos = criteria.MeetCriteria(allPhotos);

                    criteria = new DateToCriteria(searchParams.DateTo);
                    allPhotos = criteria.MeetCriteria(allPhotos);

                    criteria = new SizeFromCriteria(searchParams.SizeFrom);
                    allPhotos = criteria.MeetCriteria(allPhotos);

                    criteria = new SizeToCriteria(searchParams.SizeTo);
                    allPhotos = criteria.MeetCriteria(allPhotos);

                    photos = allPhotos.ToList();
                    ViewBag.SearchParams = searchParams;

                }
                else
                {
                    photos = (await _photoRepo.GetAllPhotos()).ToList()
                        .OrderByDescending(x => x.UploadDate)
                        .Take(10)
                        .OrderByDescending(x => x.UploadDate)
                        .ToList();
                    ViewBag.SearchParams = new SearchParamsVM();
                }



                foreach (var photo in photos)
                {
                    var user = await _userRepo.GetUserAsync(photo.UserId);
                    var hashtags = await _photoRepo.GetHashtagsForPhoto(photo.Id);

                    explorePhotos.Add(new ExplorePhotoVM(photo, user, hashtags));
                }

                var authors = await _userRepo.GetAllUsers();
                ViewBag.Authors = authors;

                return IsUserLogged ? View(explorePhotos) : View("ExplorePublic", explorePhotos);
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [HttpPost]
        [Route("download")]
        public async Task<IActionResult> DownloadPhoto([FromForm] PhotoDownloadVM photoDownload)
        {
            try
            {
                var photo = await _photoRepo.GetPhotoAsync(photoDownload.PhotoId);

                var net = new System.Net.WebClient();
                var photoUrl = "https://cp0fs.blob.core.windows.net/cloudpic/" + photo.Guid + "." + photo.Format;
                var data = net.DownloadData(photoUrl);

                var finalPhoto = _photoService.DownloadPhoto(data, photoDownload);

                var content = new System.IO.MemoryStream(finalPhoto);
                var contentType = "image/" + (photoDownload.Format != "jpeg" ? photoDownload.Format : "jpg");
                var fileName = photo.Guid + "." + photoDownload.Format;
                return File(content, contentType, fileName);
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }
        #endregion

    }
}
