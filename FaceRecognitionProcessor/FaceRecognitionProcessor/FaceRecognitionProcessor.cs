using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FaceRecognitionProcessor
{
    public static class FaceRecognitionProcessor
    {
        private readonly static string _subscriptionKey = "00c1f702d0fb4fab80dfabe5abfe404c";

        [FunctionName("FaceRecognitionProcessor")]
        public static void Run([BlobTrigger("imagecontainer/{name}", Connection = "StorageConnection")]Stream stream, string name,
             [DocumentDB("BootcampDB", "ImageData", ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
              TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {stream.Length} Bytes");
            var results = AnalyzeImage(stream).Result;
            document = JsonConvert.SerializeObject(new { results = results });
        }

        private static async Task<JArray> AnalyzeImage(Stream stream)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            string requestParameters = "returnFaceId=false&returnFaceLandmarks=false&returnFaceAttributes=age,gender,smile,facialHair,glasses,emotion,hair,makeup,accessories";

            string uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect" + "?" + requestParameters;

            HttpResponseMessage response;
            byte[] byteData = stream.ReadAllBytes();

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                return JArray.Parse(await response.Content.ReadAsStringAsync());
            }
        }

        private static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
