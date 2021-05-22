using CloudPic.BAL.Interfaces;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using CloudPic.Models.VM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostSharp.Patterns.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class LogService : ILogService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogRepo _logRepo;

        public LogService(IConfiguration configuration,
            ILogRepo logRepo)
        {
            this._configuration = configuration;
            this._logRepo = logRepo;
        }

        [Cache]
        public async Task<IEnumerable<LogDetailsVM>> GetAllLogs() => await _logRepo.GetAllLogs();


        [InvalidateCache(nameof(GetAllLogs))]
        public async Task<object> InsertLog(Log log) => await _logRepo.InsertLog(log);

    }
}
