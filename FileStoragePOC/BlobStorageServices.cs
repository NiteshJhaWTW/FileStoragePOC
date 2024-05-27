using Azure.Storage.Blobs;
using FileStoragePOC.Enums;
using FileStoragePOC.Models;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStoragePOC
{
    public class BlobStorageServices
    {
        private readonly ILogger _logger;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly AsyncRetryPolicy _retryPolicy;


        public BlobStorageServices(ILogger<BlobStorageServices> logger
            , BlobContainerClient blobContainerClient
            , AsyncRetryPolicy retryPolicy)
        {
            _logger = logger;
            _blobContainerClient = blobContainerClient;
            _retryPolicy = retryPolicy;
        }


        public async Task UploadAsync(ExportFile exportFile)
        {

            //string blobName = GetBlobPath(exportFile.FileType, exportFile.FileName);
            string blobName = "badFile" + "/" + exportFile.FileName;
            var blobClient = _blobContainerClient.GetBlobClient(blobName);


            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream(exportFile.FileContent))
                    {
                        await blobClient.UploadAsync(stream, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Failed to upload blob ");
                    throw;
                }

            });

        }

        private string GetBlobPath(ExportFileType fileType, string fileName)
        {
            string offshoreFolder = "Acclaris", onshoreFolder = "Acclaris Onshore-Only";
            string blobName;
            if (fileType == ExportFileType.OffShore)
            {
                blobName = offshoreFolder + "/" + fileName;
            }
            else
            {
                blobName = onshoreFolder + "/" + fileName;
            }

            return blobName;
        }
    }
}
