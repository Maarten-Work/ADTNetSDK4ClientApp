using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Helpers;
using Newtonsoft.Json;
using System.Net;

// See https://aka.ms/new-console-template for more information
var adtInstanceUrl = $"https://dgtwiniothack.api.weu.digitaltwins.azure.net";

InteractiveBrowserCredentialOptions credentialOptions = new InteractiveBrowserCredentialOptions()
           {
               TenantId = $"72f988bf-86f1-41af-91ab-2d7cd011db47"           };
 
var credential = new InteractiveBrowserCredential(credentialOptions);
var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);

Console.WriteLine($"ADT service client created - ready to go!");

Console.WriteLine();
Console.WriteLine($"Upload a model");

var dtdl = File.ReadAllText("SampleModel.json");
var models = new List<string> { dtdl };
var deserializedModel = JsonConvert.DeserializeObject<IDictionary<string,object>>(dtdl);

try 
{
    // check if model already exists    
    Response<DigitalTwinsModelData> model =   await client.GetModelAsync(deserializedModel?["@id"] as String);
    // Model is already there 
    Console.WriteLine("Model is already uploaded, skipping upload");
    Console.WriteLine("In case you want to update the model, consider: ");
    Console.WriteLine("     1. Delete the old model and upload a new one.");
    Console.WriteLine("     2. Upload a new version of the model.");
    Console.WriteLine("");


}
catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
{

    // Upload the model to the ADT service instance
    try 
    {
        await client.CreateModelsAsync(models);
        Console.WriteLine($"Models uploaded to the ADT service instance.");
    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"Upload model error: {e.Status}: {e.Message}");
    }
}



// Read a list of models back from the ADT service instance
var modelDataList = client.GetModelsAsync();
await foreach (var md in modelDataList)
{
    Console.WriteLine($"Model: {md.Id}");
}


var twinData = new BasicDigitalTwin();
twinData.Metadata.ModelId = deserializedModel["@id"] as String;
twinData.Contents.Add("data", $"Hello World");


// Create 3 sample Digital Twins
var prefix = "sampleTwin-";
for (int i = 0; i < 3; i++)
{
    try
    {
        twinData.Id = $"{prefix}{i}";
        await client.CreateOrReplaceDigitalTwinAsync<BasicDigitalTwin>(twinData.Id, twinData);
        Console.WriteLine($"Created twin: {twinData.Id}");
    }
    catch(RequestFailedException e)
    {
        Console.WriteLine($"Create twin error: {e.Status}: {e.Message}");
    }
}

// Create Relationships between twins 
await Relationship.CreateRelationshipAsync(client, "sampleTwin-0", "sampleTwin-1");
await Relationship.CreateRelationshipAsync(client, "sampleTwin-0", "sampleTwin-2");

await Relationship.ListRelationshipsAsync(client, "sampleTwin-0");

try
{
    var queryResult = client.QueryAsync<BasicDigitalTwin>("Select * FROM digitaltwins");

    await foreach (var twin in queryResult)
    {
        Console.WriteLine("------------------");
        Console.WriteLine($"Twin {twin.Id}");
        Console.WriteLine(JsonConvert.SerializeObject(twin));
        Console.WriteLine("------------------");
    }
}
catch (RequestFailedException e)
{
    Console.WriteLine($"Query twin error: {e.Status}: {e.Message}");
}
