
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Text;
using System.Net;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FaceRecognition
{
    public static class FaceRecognition
    {
        [FunctionName("FaceRecognition")]
        public async static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string fileName = await CreateBlob(await req.Content.ReadAsStreamAsync());


            //Http 202 Accepted with Location Uri
            return req.CreateResponse(HttpStatusCode.Accepted, new { resultUri = "https://bootcamp-cosmosbindingvs.azurewebsites.net/api/image/" + fileName } );
            }
       
        private async static Task<string> CreateBlob(Stream data)
        {
            string fileName = DateTime.Now.ToString("mmss") + "-" + Guid.NewGuid().ToString();

            string connectionString = ConfigurationManager.AppSettings["StorageConnection"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("imagecontainer");
           
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.GetBlockBlobReference(fileName + ".png");
            blob.Properties.ContentType = "image/png";
            await blob.UploadFromStreamAsync(data);

            return fileName;
        }
    }
}
