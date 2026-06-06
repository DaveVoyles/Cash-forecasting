using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunctionsApp
{
    public static class FunctionValidate
    {
        [FunctionName("FunctionValidate")]
        public static async void Run([BlobTrigger("datauploaded/{name}", Connection = "blobconnection")] Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var connectionString = Environment.GetEnvironmentVariable("blobconnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("blobconnection is not configured.");
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("datavalidated");
            log.Info("New container name" + container.Name);
            var blockBlob = container.GetBlockBlobReference(name);

            log.Info($"BlockBlob destination name: {blockBlob.Name}");
            log.Info("Starting Copy");

            try
            {
                await blockBlob.UploadFromStreamAsync(myBlob);
                log.Info("Copy completed");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Info("Copy failed");
            }
            finally
            {
                log.Info("Operation completed");
            }
        }
    }
}
