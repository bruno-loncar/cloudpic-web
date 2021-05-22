using System;

namespace CloudPic.Models.DTO
{
    public class UserPlan
    {
        public UserPlan(int id, int userId, int planId, DateTime dateFrom, DateTime? dateTo)
        {
            Id = id;
            UserId = userId;
            PlanId = planId;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public UserPlan(int userId, int planId, DateTime dateFrom, DateTime? dateTo)
            : this(0, userId, planId, dateFrom, dateTo)
        {
        }

        public int Id { get; }
		public int UserId { get; }
		public int PlanId { get; }
		public DateTime DateFrom { get; }
		public DateTime? DateTo { get; }

	}
}
