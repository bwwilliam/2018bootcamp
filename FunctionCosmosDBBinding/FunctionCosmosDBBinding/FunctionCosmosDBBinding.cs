using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;


namespace FunctionCosmosDBBinding
{
    public static class FunctionCosmosDBBinding
    {

        [FunctionName("FunctionCosmosDBBinding")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{id}")]HttpRequestMessage req, string id, TraceWriter log,
              [DocumentDB("BootcampDB", "ImageData", SqlQuery = "select * from c where c.id = {id}", ConnectionStringSetting = "CosmosDBConnection")]IEnumerable<dynamic> inputDocuments,
              [DocumentDB("BootcampDB", "ImageData", ConnectionStringSetting = "CosmosDBConnection")]out dynamic outputDocument)

        {
            var inputDoc = inputDocuments.FirstOrDefault();
            inputDoc.currentTime = DateTime.Now;
            outputDocument = inputDoc;

            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(inputDoc), Encoding.UTF8, "application/json")
            };

            return response;
        }
    }
}

