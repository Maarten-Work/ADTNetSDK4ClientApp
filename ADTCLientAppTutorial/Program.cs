using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Text.Json;
using Helpers;

// See https://aka.ms/new-console-template for more information
var adtInstanceUrl = "https://rg-adt-mst01.api.weu.digitaltwins.azure.net";

var credential = new DefaultAzureCredential();
var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);

Console.WriteLine($"ADT service client created - ready to go!");

Console.WriteLine();
Console.WriteLine($"Upload a model");

var dtdl = File.ReadAllText("SampleModel.json");
var models = new List<string> { dtdl };

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

// Read a list of models back from the ADT service instance
var modelDataList = client.GetModelsAsync();
await foreach (var md in modelDataList)
{
    Console.WriteLine($"Model: {md.Id}");
}

var twinData = new BasicDigitalTwin();
twinData.Metadata.ModelId = "dtmi:example:SampleModel;1";
twinData.Contents.Add("data", $"Hello World");

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

await Relationship.CreateRelationshipAsync(client, "sampleTwin-0", "sampleTwin-1");
await Relationship.CreateRelationshipAsync(client, "sampleTwin-0", "sampleTwin-2");

await Relationship.ListRelationshipsAsync(client, "sampleTwin-0");

try
{
    var queryResult = client.QueryAsync<BasicDigitalTwin>("Select * FROM digitaltwins");

    await foreach (var twin in queryResult)
    {
        Console.WriteLine(JsonSerializer.Serialize(twin));
        Console.WriteLine("------------------");
    }
}
catch (RequestFailedException e)
{
    Console.WriteLine($"Query twin error: {e.Status}: {e.Message}");
}
