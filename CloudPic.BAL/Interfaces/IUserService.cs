using CloudPic.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.BAL.Interfaces
{
    public interface IUserService
    {
        Task<int> DeleteUser(int id);
    }
}
