using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public class DateToCriteria : ICriteria
    {
        private readonly DateTime dateTo;

        public DateToCriteria(DateTime dateTo)
        {
            this.dateTo = dateTo;
        }

        public List<Photo> MeetCriteria(IEnumerable<Photo> photos)
        {
            if (dateTo != default)
            {
                return photos.Where(x => x.UploadDate <= this.dateTo.AddDays(1)).ToList();
            }
            return photos.ToList();
        }
    }
}
