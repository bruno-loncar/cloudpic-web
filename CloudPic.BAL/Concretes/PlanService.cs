using CloudPic.BAL.Interfaces;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class PlanService : IPlanService
    {
        private readonly ILogger<PlanService> _logger;
        private readonly IConfiguration _configuration;

        public PlanService(ILogger<PlanService> logger,
            IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

    }
}
