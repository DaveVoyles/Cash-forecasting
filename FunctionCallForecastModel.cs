using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;

namespace FunctionsApp
{
    public static class FunctionCallForecastModel
    {
        [FunctionName("FunctionCallForecastModel")]
        public async static void Run([BlobTrigger("datavalidated/{name}", Connection = "blobconnection")]Stream myBlob, string name, string blobTrigger, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            //Add URI to app settings
            string blobPath = "https://forcastdatastracc.blob.core.windows.net/" + blobTrigger;

            var client = new HttpClient();
            var parameters = new Dictionary<string, string> { { "filename", blobPath } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            client.BaseAddress = new Uri("http://pncdsvm.eastus.cloudapp.azure.com:5000/forecast");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsync("http://pncdsvm.eastus.cloudapp.azure.com:5000/forecast", encodedContent).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();

            log.Info(response.ToString());
            log.Info("Result - " + content);
        }
    }
}
