using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public class SizeToCriteria : ICriteria
    {
        private readonly int sizeTo;

        public SizeToCriteria(int sizeTo)
        {
            this.sizeTo = sizeTo;
        }

        public List<Photo> MeetCriteria(IEnumerable<Photo> photos)
        {
            if (sizeTo != default)
            {
                return photos.Where(x => x.SizeInMb <= sizeTo).ToList();
            }
            return photos.ToList();
        }
    }
}
