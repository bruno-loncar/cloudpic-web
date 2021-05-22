using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.Models.Util
{
    public class AzureContainerClient : BlobContainerClient
    {
        private static BlobContainerClient _containerClient;

        public static BlobContainerClient GetInstance(IConfiguration _configuration)
        {
            if (_containerClient == null)
            {
                var _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("CloudPic_FS"));
                var containerClient = _blobServiceClient.GetBlobContainerClient(_configuration["FileShare:BlobFSContainer"]);
                _containerClient = containerClient;
            }
            return _containerClient;
        }
    }
}
