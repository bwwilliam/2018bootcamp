# 2018 Microsoft Azure Global Bootcamp

## Develop & Deploy Azure Functions using Visual Studio

Please follow the instructions [https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs) 
* Setup Azure Development workload on Visual Studio 2017
* Update Azure Functions and Web Jobs Tools
* Create an Azure Functions App
* Deploy Azure Functions


## Setup CosmosDB using Azure Portal

Please follow the instructions [https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet](https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet) 
* Create a database account
* Add a collection
* Add sample data
* Query your data

Ignore the rest of the sections on this page


## Integrate CosmosDB with Azure Functions
* Add Nuget package <code>Microsoft.Azure.WebJobs.Extensions.DocumentDB</code> to Azure Functions App
* Add <code>DocumentDB</code> binding <code>Run</code> method
```csharp
public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "myapi/{id}")]HttpRequestMessage req, string id, TraceWriter log,
              [DocumentDB("CosmosDB", "CosmosDBCollection", SqlQuery = "select * from c where c.id = {id}", ConnectionStringSetting = "CosmosDBConnection")]IEnumerable<dynamic> inputDocuments,
              [DocumentDB("CosmosDB", "CosmosDBCollection", ConnectionStringSetting = "CosmosDBConnection")]out dynamic outputDocument)

{
    var inputDoc = inputDocuments.FirstOrDefault();
    outputDocument = inputDoc;
    var response = new HttpResponseMessage()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(JsonConvert.SerializeObject(inputDoc), Encoding.UTF8, "application/json")
    };
    return response;
}
```

