using System;

namespace CloudPic.Models.DTO
{
    public class Hashtag
    {
        public Hashtag(string name, int photoId)
            : this (0, name, photoId)
        {

        }

        public Hashtag(int id, string name, int photoId)
        {
            Id = id;
            Name = name;
            PhotoId = photoId;
        }

        public int Id { get; }
        public string Name { get; }
        public int PhotoId { get; }
    }
}
