using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class PlanRepo : DbRepo, IPlanRepo
    {
        public async Task<object> InsertUserPlanAsync(UserPlan userPlan) => await DBConn.InsertAsync(userPlan);

        public async Task<int> ChangePackageAsync(int id, int packageId) => await DBConn.SingleAsync<int>(";EXEC ChangePackage @0, @1", id, packageId);

        public async Task<Plan> GetPlan(int id) => await DBConn.SingleOrDefaultAsync<Plan>("where id = @0", id);
        public async Task<UserPlan> GetLastPackage(int id) => await DBConn.SingleOrDefaultAsync<UserPlan>("where userId = @0 AND dateTo IS NULL", id);
        public async Task<ConsumationOnDateVM> GetConsumeOnDate(int id, DateTime date) => 
            await DBConn.SingleOrDefaultAsync<ConsumationOnDateVM>(";EXEC GetConsumeOnDate @0, @1", id, date);

    }
}
