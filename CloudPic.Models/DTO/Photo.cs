using System;
using System.Threading.Tasks;

namespace CloudPic.Models.DTO
{
    public class Photo
    {
        #region Constructors

        public Photo(int id, string guid, int userId, DateTime uploadDate, string description, string format, double sizeInMb)
        {
            Id = id;
            Guid = guid;
            UserId = userId;
            UploadDate = uploadDate;
            Description = description;
            Format = format;
            SizeInMb = sizeInMb;
        }

        #endregion


        #region Properties

        public int Id { get; }
		public string Guid { get; }
		public int UserId { get; }
		public DateTime UploadDate { get; }
		public string Description { get; }
		public string Format { get; }
		public double SizeInMb { get; }

        #endregion

        public string GetImageUrl()
        {
            return $"https://cp0fs.blob.core.windows.net/cloudpic/{Guid}.{Format}";
        }

        public string GetThumbnailUrl()
        {
            return $"https://cp0fs.blob.core.windows.net/cloudpic/thumbnails/{Guid}.{Format}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Id == (obj as Photo).Id;    
        }

        public override int GetHashCode()
        {
            return Id % 7;
        }

    }
}
