using ImageProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloudPic.BAL.Prototype
{
    public class Image : IPrototype<Image>
    {
        public byte[] Data { get; }

        public Image(byte[] data)
        {
            this.Data = data;
        }

        public Image Clone()
        {
            return new Image(Data);
        }

        internal Image GaussianBlur(int gaussianBlurIntensity)
        {
            using (MemoryStream outStreamImage = new MemoryStream())
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(Data)
                        .GaussianBlur(gaussianBlurIntensity)
                        .Save(outStreamImage);

                }
                return new Image(outStreamImage.ToArray());
            }
        }

        internal Image GaussianSharpen(int gaussianSharpenIntensity)
        {
            using (MemoryStream outStreamImage = new MemoryStream())
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(Data)
                        .GaussianSharpen(gaussianSharpenIntensity)
                        .Save(outStreamImage);

                }
                return new Image(outStreamImage.ToArray());
            }
        }
        
        internal Image Saturation(int saturationIntensity)
        {
            using (MemoryStream outStreamImage = new MemoryStream())
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(Data)
                        .Saturation(saturationIntensity)
                        .Save(outStreamImage);

                }
                return new Image(outStreamImage.ToArray());
            }
        }
    }
}
