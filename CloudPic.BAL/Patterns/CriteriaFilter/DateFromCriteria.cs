using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public class DateFromCriteria : ICriteria
    {
        private readonly DateTime dateFrom;

        public DateFromCriteria(DateTime dateFrom)
        {
            this.dateFrom = dateFrom;
        }

        public List<Photo> MeetCriteria(IEnumerable<Photo> photos)
        {
            if (dateFrom != default)
            {
                return photos.Where(x => x.UploadDate >= this.dateFrom).ToList();
            }
            return photos.ToList();
        }
    }
}
