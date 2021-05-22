using CloudPic.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Interfaces
{
    public interface IUserRepo
    {
        Task<User> GetUserAsync(int id);
        Task<User> GetUserAsync(string email);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> LoginUserAsync(string email, string password);
        Task<object> InsertUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task<int> DeleteUserAsync(int id);
        Task<int> DeleteUserPlansForUser(int id);
    }
}
