using System;

namespace CloudPic.Models.DTO
{
    public class Plan
    {
        public Plan(int id, string name, decimal pricePerDay, int photosPerDay, int storageSize, int mbPerDay)
        {
            Id = id;
            Name = name;
            PricePerDay = pricePerDay;
            PhotosPerDay = photosPerDay;
            StorageSize = storageSize;
            MbPerDay = mbPerDay;
        }

        public int Id { get; }
		public string Name { get; }
		public decimal PricePerDay { get; }
		public int PhotosPerDay { get; }
		public int StorageSize { get; }
		public int MbPerDay { get; }

        public static int Parse(string package)
        {
            return (package.ToLower()) switch
            {
                "free" => 1,
                "pro" => 2,
                "gold" => 3,
                _ => throw new Exception("Input package is not valid"),
            };
        }
    }
}
