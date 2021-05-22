using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.Models.VM
{
    public class PhotoHashtagVM
    {
        public PhotoHashtagVM(Photo photo, List<Hashtag> hashtags)
        {
			Id = photo.Id;
			Guid = photo.Guid;
			UserId = photo.UserId;
			UploadDate = photo.UploadDate;
			Description = photo.Description;
			Format = photo.Format;
			SizeInMb = photo.SizeInMb;

            Hashtags = string.Join(", ", hashtags.Select(x => x.Name));
		}

		public int Id { get; }
		public string Guid { get; }
		public int UserId { get; }
		public DateTime UploadDate { get; }
		public string Description { get; }
		public string Format { get; }
		public double SizeInMb { get; }
		public string Hashtags { get; }

		public string GetImageUrl()
		{
			return $"https://cp0fs.blob.core.windows.net/cloudpic/{Guid}.{Format}";
		}

		public string GetThumbnailUrl()
		{
			return $"https://cp0fs.blob.core.windows.net/cloudpic/thumbnails/{Guid}.{Format}";
		}
	}
}
