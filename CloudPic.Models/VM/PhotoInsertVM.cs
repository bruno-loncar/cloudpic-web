using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class PhotoInsertVM
    {
        public string Description { get; set; } = "";
        public int Width { get; set; }
        public int Height { get; set; }
        public string Tags { get; set; } = "";
        public string Format { get; set; } = "";
        public IFormFile File { set; get; }
    }
}
