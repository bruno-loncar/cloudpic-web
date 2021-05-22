using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class PhotoDownloadVM
    {
        public int PhotoId { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public string Format { get; set; } = "";

        public double GaussianBlurIntensity { get; set; }
        public double GaussianSharpenIntensity { get; set; }
        public double SaturationIntensity { get; set; }
    }
}
