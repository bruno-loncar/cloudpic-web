using CloudPic.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Interfaces
{
    public interface IPhotoRepo
    {
        Task<Photo> GetPhotoAsync(int id);
        Task<IEnumerable<Photo>> GetAllPhotos();
        Task<IEnumerable<Photo>> GetPhotosForUserAsync(int id);
        Task<IEnumerable<Photo>> GetPhotosHashtag(string hashtag);
        Task<object> InsertPhotoAsync(Photo photo);
        Task<int> DeletePhotoAsync(int id);
        Task<int> UpdatePhotoAsync(int id, string description);

        Task<object> InsertHashTagAsync(Hashtag hashtag);
        Task<IEnumerable<Hashtag>> GetHashtagsForPhoto(int id);
        Task DeleteHashtagsForPhotoAsync(int id);
    }
}
