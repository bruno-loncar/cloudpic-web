using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class LogRepo : DbRepo, ILogRepo
    {
        public async Task<object> InsertLog(Log log) => await DBConn.InsertAsync(log);
        
        public async Task<IEnumerable<LogDetailsVM>> GetAllLogs() => await DBConn.FetchAsync<LogDetailsVM>(";EXEC GetAllLogs");
    }
}
