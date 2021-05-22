using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.VM
{
    public class FileVM
    {
        public FileVM(byte[] content, string contentType, string identifier, string extension)
        {
            Content = content;
            ContentType = contentType;
            Identifier = identifier;
            Extension = extension;
        }

        public byte[] Content { get; }
        public string ContentType { get; }
        public string Identifier { get; }
        public string Extension { get; }
    }
}
