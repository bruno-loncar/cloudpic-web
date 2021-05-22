using CloudPic.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPic.Models.VM
{
    public class PhotoUpdateVM
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
    }
}
