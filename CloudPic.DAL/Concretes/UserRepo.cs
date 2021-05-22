using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class UserRepo : DbRepo, IUserRepo
    {
        public async Task<User> GetUserAsync(int id) => await DBConn.SingleOrDefaultByIdAsync<User>(id);

        public async Task<User> GetUserAsync(string email) => await DBConn.SingleOrDefaultAsync<User>("where email = @0", email);

        public async Task<IEnumerable<User>> GetAllUsers() => await DBConn.FetchAsync<User>();

        public async Task<User> LoginUserAsync(string email, string password) => 
            await DBConn.SingleOrDefaultAsync<User>("where email = @0 and password = @1", email, password);

        public async Task<object> InsertUserAsync(User user) => await DBConn.InsertAsync(user);

        public async Task<int> UpdateUserAsync(User user) => await DBConn.UpdateAsync(user);

        public async Task<int> DeleteUserAsync(int id) => await DBConn.DeleteAsync(DBConn.SingleById<User>(id));

        public async Task<int> DeleteUserPlansForUser(int id) => await DBConn.SingleAsync<int>(";EXEC DeleteUserPlansForUser @0", id);

    }
}
