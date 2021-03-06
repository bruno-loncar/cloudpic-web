using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.BAL.Interfaces
{
    public interface ILogService
    {
        Task<object> InsertLog(Log log);
        Task<IEnumerable<LogDetailsVM>> GetAllLogs();
    }
}
