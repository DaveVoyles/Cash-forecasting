using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UploadToStorage
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            TraceWriter log)
        {
            var contentType = req.Content.Headers?.ContentType?.MediaType;
            if (contentType != "application/json")
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, string.Empty);
            }

            var body = await req.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, string.Empty);
            }

            var name = Guid.NewGuid().ToString("n");
            await CreateBlob(name + ".json", body, log);
            return req.CreateResponse(HttpStatusCode.OK, string.Empty);
        }

        private static async Task CreateBlob(string name, string data, TraceWriter log)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("AzureWebJobsStorage is not configured.");
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("testing123");

            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "application/json";

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }
    }
}
