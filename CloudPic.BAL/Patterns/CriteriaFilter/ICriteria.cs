using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public interface ICriteria
    {
        public List<Photo> MeetCriteria(IEnumerable<Photo> photos);
    }
}
