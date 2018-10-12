using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
namespace FunctionsApp
{
    public static class FunctionValidate
    {
        [FunctionName("FunctionValidate")]
        public async static void Run([BlobTrigger("datauploaded/{name}", Connection = "blobconnection")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            // Move connectionstring to app settings
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName=forcastdatastracc;AccountKey=3AlB7+NszzKhPF2lEHXQk06eE1ef8llbWF3m5d3A3LC5MeN6OB24qlMCXUEhHnsEDDmR0JJ/zQAh8E0uq7rttg==;EndpointSuffix=core.windows.net";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("datavalidated");
            log.Info("New container name" + container.Name);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

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
