using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class ExplorePhotoVM
    {
        public ExplorePhotoVM(Photo photo, User user, IEnumerable<Hashtag> hashtags)
        {
            Photo = photo;
            User = user;
            Hashtags = hashtags;
        }

        public Photo Photo { get; }
        public User User { get; }
        public IEnumerable<Hashtag> Hashtags { get; }
    }
}
