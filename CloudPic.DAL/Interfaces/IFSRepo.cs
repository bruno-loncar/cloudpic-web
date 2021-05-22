using CloudPic.Models.VM;
using System.Threading.Tasks;

namespace CloudPic.DAL.Interfaces
{
    public interface IFSRepo
    {
        Task<byte[]> GetFileAsync(string path);
        Task<int> PostFileAsync(FileVM file);
        Task<int> DeleteFileAsync(FileVM file);
    }
}
