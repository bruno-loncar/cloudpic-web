using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.Models.VM
{
    public class LogDetailsVM
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LogType { get; set; }
        public int ObjectId { get; set; }
        public DateTime LogDate { get; set; }
    }
}
