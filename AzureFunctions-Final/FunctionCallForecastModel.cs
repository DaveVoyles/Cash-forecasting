using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Functions
{
    public static class FunctionCallForecastModel
    {
        [FunctionName("FunctionCallForecastModel")]
        public async static void Run([BlobTrigger("datavalidated/{name}", Connection = "blobconnection")]Stream myBlob, string name, string blobTrigger, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            //Add URI to app settings
            string blobPath = Environment.GetEnvironmentVariable("blobBaseURI") + blobTrigger;

            string result = await CallForecastModel(blobPath);
            log.LogInformation("Result - " + result);

            SaveResultToBlob(result,name,log);

        }

        private static void SaveResultToBlob(string result, string fileName, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("blobconnection");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("dataresult");
            log.LogInformation("New container name" + container.Name);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("Output-"+fileName.Replace("csv","txt"));

            log.LogInformation($"BlockBlob destination name: {blockBlob.Name}");
            log.LogInformation("Starting Copy");

            try
            {
                blockBlob.UploadText(result);
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

        private async static Task<string> CallForecastModel(string blobPath)
        {
            var client = new HttpClient();
            var parameters = new Dictionary<string, string> { { "filename", blobPath } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ModelEndpoint"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsync(Environment.GetEnvironmentVariable("ModelEndpoint"), encodedContent).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}
