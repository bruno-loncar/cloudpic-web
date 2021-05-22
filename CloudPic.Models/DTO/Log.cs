using CloudPic.Models.Enums;
using System;

namespace CloudPic.Models.DTO
{
    public class Log
    {
        public Log(int id, int userId, int logTypeId, int objectId, DateTime logDate)
        {
            Id = id;
            UserId = userId;
            LogTypeId = logTypeId;
            ObjectId = objectId;
            LogDate = logDate;
        }

        public Log(int userId, LogType logType, int objectId)
        {
            UserId = userId;
            LogTypeId = (int)logType;
            ObjectId = objectId;
            LogDate = DateTime.Now;
        }

        public int Id { get; }
		public int UserId { get; }
		public int LogTypeId { get; }
		public int ObjectId { get; }
		public DateTime LogDate { get; }
	}
}
