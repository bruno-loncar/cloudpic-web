using CloudPic.DAL.Concretes;
using ImageProcessor.Imaging.Formats;
using NUnit.Framework;
using PostSharp.Patterns.Caching;

namespace CloudPic.Tests.UnitTests
{
    public class PhotosTests
    {
        [SetUp]
        public void Setup()
        {
            CachingServices.DefaultBackend = new PostSharp.Patterns.Caching.Backends.MemoryCachingBackend();
        }

        [Test]
        public void ParseImageFormatTest()
        {
            var parsedFormat = PhotoService.ParseImageFormat("jpg");
            Assert.AreEqual(new JpegFormat().ImageFormat, parsedFormat.ImageFormat);

            parsedFormat = PhotoService.ParseImageFormat("png");
            Assert.AreEqual(new PngFormat().ImageFormat, parsedFormat.ImageFormat);

            parsedFormat = PhotoService.ParseImageFormat("gif");
            Assert.AreEqual(new GifFormat().ImageFormat, parsedFormat.ImageFormat);
        }

        [Test]
        public void ConvertBytesToMegabytesTest()
        {
            var convertedMegabytes = PhotoService.ConvertBytesToMegabytes(4096);
            Assert.AreEqual(0.00390625, convertedMegabytes);

            convertedMegabytes = PhotoService.ConvertBytesToMegabytes(4096);
            Assert.AreEqual(0.00390625, convertedMegabytes);

            convertedMegabytes = PhotoService.ConvertBytesToMegabytes(32768);
            Assert.AreEqual(0.03125, convertedMegabytes);
        }
    }
}