using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public class SizeFromCriteria : ICriteria
    {
        private readonly int sizeFrom;

        public SizeFromCriteria(int sizeFrom)
        {
            this.sizeFrom = sizeFrom;
        }

        public List<Photo> MeetCriteria(IEnumerable<Photo> photos)
        {
            if (sizeFrom != default)
            {
                return photos.Where(x => x.SizeInMb >= sizeFrom).ToList();
            }
            return photos.ToList();
        }
    }
}
