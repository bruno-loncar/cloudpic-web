using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class PhotoRepo : DbRepo, IPhotoRepo
    {
        public async Task<Photo> GetPhotoAsync(int id) => await DBConn.SingleByIdAsync<Photo>(id);

        public async Task<IEnumerable<Photo>> GetAllPhotos() => await DBConn.FetchAsync<Photo>();

        public async Task<IEnumerable<Photo>> GetPhotosForUserAsync(int id) => await DBConn.FetchAsync<Photo>("where userId = @0", id);

        public async Task<IEnumerable<Photo>> GetPhotosHashtag(string hashtag) => await DBConn.FetchAsync<Photo>(";EXEC GetAllPhotosHashtag @0", hashtag);

        public async Task<object> InsertPhotoAsync(Photo photo) => await DBConn.InsertAsync(photo);

        public async Task<int> DeletePhotoAsync(int id) => await DBConn.DeleteAsync(DBConn.SingleById<Photo>(id));

        public async Task<int> UpdatePhotoAsync(int id, string description) => await DBConn.SingleAsync<int>(";EXEC UpdatePhoto @0, @1", id, description);


        public async Task<object> InsertHashTagAsync(Hashtag hashtag) => await DBConn.InsertAsync(hashtag);
        public async Task<IEnumerable<Hashtag>> GetHashtagsForPhoto(int id) => await DBConn.FetchAsync<Hashtag>("where photoId = @0", id);
        public async Task DeleteHashtagsForPhotoAsync(int id) => (await GetHashtagsForPhoto(id)).ToList().ForEach(async x => await DBConn.DeleteAsync(x));

    }

}
