using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Interfaces
{
    public interface ILogRepo
    {
        Task<object> InsertLog(Log log);

        Task<IEnumerable<LogDetailsVM>> GetAllLogs();
    }
}
