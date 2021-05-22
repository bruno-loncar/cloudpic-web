using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.BAL.CriteriaFilter
{
    public class AuthorCriteria : ICriteria
    {
        private readonly int authorId;

        public AuthorCriteria(int authorId)
        {
            this.authorId = authorId;
        }

        public List<Photo> MeetCriteria(IEnumerable<Photo> photos)
        {
            if (authorId > 0)
            {
                return photos.ToList().Where(x => x.UserId == authorId).ToList();
            }
            return photos.ToList();
        }
    }
}
