using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class SearchParamsVM
    {
        public string Tags { get; set; }
        public int AuthorId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int SizeFrom { get; set; }
        public int SizeTo { get; set; }

        public SearchParamsVM()
        {
            Tags = "";
            AuthorId = -1;
            DateFrom = DateTime.Today.AddDays(-30);
            DateTo = DateTime.Today;
            SizeFrom = 0;
            SizeTo = 10;
        }

        public bool IsInitialized()
        {
            return !(Tags == default &&
                   AuthorId < 1 &&
                   DateFrom == default &&
                   DateTo == default &&
                   SizeFrom == default &&
                   SizeTo == default);
        }
    }
}
