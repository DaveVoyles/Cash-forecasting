using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace UploadToStorage
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            HttpStatusCode result;
            string contentType;

            result = HttpStatusCode.BadRequest;

            contentType = req.Content.Headers?.ContentType?.MediaType;

            if (contentType == "application/json")
            {
                string body;

                body = await req.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(body))
                {
                    string name;

                    name = Guid.NewGuid().ToString("n");

                    await CreateBlob(name + ".json", body, log);

                    result = HttpStatusCode.OK;
                }
            }
            return req.CreateResponse(result, string.Empty);
        }
        private async static Task CreateBlob(string name, string data, TraceWriter log)
        {
            string connStorageConnection = "DefaultEndpointsProtocol=https;AccountName=pnc;AccountKey=KH8H87QPIkk6jJ9aAOzUahsTsioDldINFrYnhUc46aWm8aA+NM1BDj81H5yHvxle2EC8TAT18gT6tiTnBH70PQ==;EndpointSuffix=core.windows.net";
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStorageConnection);


            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference("testing123");

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "application/json";

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }
    }
}
