
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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
using Microsoft.Extensions.Primitives;

namespace FaceRecognition
{
    public static class FaceRecognition
    {
        [FunctionName("FaceRecognition")]
        public async static Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, 
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            (Uri StorageUri, Uri ResultUri) blobStatus = await CreateBlob(req.Body);

            //Http 202 Accepted with Location Uri
            return new AcceptedResult(blobStatus.StorageUri, new { resultUri = blobStatus.ResultUri } );
        }
       
        private async static Task<(Uri, Uri)> CreateBlob(Stream data)
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

            return (blob.Uri, new Uri(container.Uri, container.Name + "/" + fileName + ".json"));
        }
    }
}
