using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.Util;
using CloudPic.Models.VM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class FSRepo : DbRepo, IFSRepo
    {
        private readonly IConfiguration _configuration;

        public FSRepo(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        #region Public methods
        public async Task<byte[]> GetFileAsync(string path)
        {
            BlobContainerClient containerClient = AzureContainerClient.GetInstance(_configuration);

            var blobClient = containerClient.GetBlobClient(path);
            var blobDownloadInfo = await blobClient.DownloadAsync();

            var ms = new MemoryStream();
            blobDownloadInfo.Value.Content.CopyTo(ms);

            return ms.ToArray();
        }

        public async Task<int> PostFileAsync(FileVM file)
        {
            BlobContainerClient containerClient = AzureContainerClient.GetInstance(_configuration);

            var blobClient = containerClient.GetBlobClient($"{file.Identifier}.{file.Extension}");
            _ = await blobClient.UploadAsync(new MemoryStream(file.Content), new BlobHttpHeaders { ContentType = file.ContentType });

            return 1;
        }

        public async Task<int> DeleteFileAsync(FileVM file)
        {
            BlobContainerClient containerClient = AzureContainerClient.GetInstance(_configuration);

            var blobClient = containerClient.GetBlobClient($"{file.Identifier}.{file.Extension}");
            _ = await blobClient.DeleteAsync();

            return 1;
        }
        #endregion

    }
}
