using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class ConsumationOnDateVM
    {
        public ConsumationOnDateVM(int numberOfPhotos, int sizeInMb, int storage, DateTime date)
        {
            NumberOfPhotos = numberOfPhotos;
            SizeInMb = sizeInMb;
            Storage = storage;
            Date = date;
        }

        public int NumberOfPhotos { get; }
        public int SizeInMb { get; }
        public int Storage { get; }
        public DateTime Date { get; }
    }
}
