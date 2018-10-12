using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace FunctionsPNC
{
    public static class FunctionValidate
    {
        [FunctionName("FunctionValidate")]
        public async static void Run([BlobTrigger("datauploaded/{name}", Connection = "blobconnection")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            // Move connectionstring to app settings
            var connectionString = Environment.GetEnvironmentVariable("blobconnection");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("datavalidated");
            log.LogInformation("New container name" + container.Name);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            log.LogInformation($"BlockBlob destination name: {blockBlob.Name}");
            log.LogInformation("Starting Copy");

            try
            {
                blockBlob.UploadFromStream(myBlob);
                log.LogInformation("Copy completed");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogInformation("Copy failed");
            }
            finally
            {
                log.LogInformation("Operation completed");
            }
        }
    }
}
