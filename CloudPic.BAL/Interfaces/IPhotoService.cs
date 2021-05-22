using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.BAL.Interfaces
{
    public interface IPhotoService
    {
        Task<Photo> GetPhoto(int id);
        Task<int> DeletePhoto(int id);
        Task<int> UpdatePhoto(PhotoUpdateVM photoUpdateVM);
        Task<(int, int)> UploadPhoto(PhotoInsertVM photoInsertVM, int userId);
        Task<IEnumerable<Photo>> GetPhotosForUser(int userId);
        Task<IEnumerable<PhotoHashtagVM>> GetPhotosDetailedForUser(int userId);
        byte[] DownloadPhoto(byte[] data, PhotoDownloadVM photoDownload);
    }
}
