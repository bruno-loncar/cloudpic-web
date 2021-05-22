using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System;
using System.Threading.Tasks;

namespace CloudPic.DAL.Interfaces
{
    public interface IPlanRepo
    {
        Task<object> InsertUserPlanAsync(UserPlan userPlan);
        Task<int> ChangePackageAsync(int id, int packageId);
        Task<Plan> GetPlan(int id);
        Task<UserPlan> GetLastPackage(int id);
        Task<ConsumationOnDateVM> GetConsumeOnDate(int id, DateTime date);
    }
}
