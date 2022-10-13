# Coding with the Azure Digital Twins SDK for .NET (C#)

## Introduction

This hack is inspired by an existing [tutorial](https://learn.microsoft.com/en-us/azure/digital-twins/tutorial-code), but updated to use the latest version of .NET Core and Visual Studio Code as Development Environment. You will create a C# console client app from scratch. Not only assures excercise that you have all the necessary development tools installed, but it also helps you getting proper access to an Azure Digital Twin instance that you create as part of this task.

## Prerequisites

This hack uses the command line, both for setup and project work. This makes Visual Studio Code an ideal environment to work in.

What you need to begin:

- Visual Studio Code (or any other code editor you prefer)
- .NET Core 6.x on your development machine. You can download the latest version for multiple platforms from this [download location](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Prepare an Azure Digital Twins instance

To work with Azure Digital Twins you'll need an Azure Digital Twins instance and you need the required permissions for using it. You can follow [these instructions](https://learn.microsoft.com/en-us/azure/digital-twins/how-to-set-up-instance-portal) to do so in the Azure portal. If you rather use the CLI, you can follow [these instructions](https://learn.microsoft.com/en-us/azure/digital-twins/how-to-set-up-instance-cli).
Make sure you have an AAD user defined that you can assign to the **Azure Digital Twins Data Owner** role.

## Create a .NET console application

Run the following steps to create a new .NET console application and to add some dependencies to your project.

1) Create a new .NET console app in a new folder (e.g. ADTClientAppTutorial)
1) Add dependencies for the following packages to your project: `Azure.DigitalTwins.Core` and `Azure.Identity`

> NOTE: The complete sample code can be found in this repository inside the *ADTClientAppTutorial* folder.

### Authenticate against the ADT service

The first thing you need to do in an application when you want to use SDK functions is to authenticate the application against the ADT service. To authenticate you need the host name of your ADT instance.

Add the following using statements to your Program.cs file:

```csharp
using Azure.DigitalTwins.Core;
using Azure.Identity;
```

and in your main methode, add the following code for authentication:

```csharp
string adtInstanceUrl = "https://<your-ADT-instance-hostname>";

var credential = new DefaultAzureCredential();
var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);
Console.WriteLine($"ADT service client created - ready to go");
```

### Upload a model

You will use a DTDL model to define the types of elements in your ADT service instance. A DTDL model can be stored in a JSON-like language. Create a new json file in your project, named *SampleModel.json* and paste in the following code:

```json
{
    "@id": "dtmi:example:SampleModel;1",
    "@type": "Interface",
    "displayName": "SampleModel",
    "contents": [
        {
            "@type": "Relationship",
            "name": "Contains"
        },
        {
            "@type": "Property",
            "name": "data",
            "schema": "string"
        }
    ],
    "@context": "dtmi:dtdl:context;2"
}
``` 

Now you will add some more code to Program.cs to upload the model that you just created into your ADT service instance.

Begin with adding a few more using statements:

```csharp
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Azure;
```

Now add the following code to your Main class to load the DTDL file you created earlier and to upload the model to your ADT service instance:

```csharp
Console.Writeline();
Console.Writeline($"Upload a model");

var dtdl = File.ReadAllText("SampleModel.json");
var models = new List<string> { dtdl };

// Upload the model to the ADT service instance
await client.CreateModelsAsync(models);
```

To verify that uploading the model was succesful, also add the following code:

```csharp
// Read a list of models back from the ADT service instance
var modelDataList = client.GetModelsAsync();
await foreach (var md in modelDataList)
{
    Console.WriteLine($"Model: {md.Id}");
}
```

If you execute the application you will see that the model is created in your ADT service instance. However, if you execute the application again, an exception will be thrown because the Model already exists. So you need to add exception handling to your code to prevent this exception from being raised.

### Create digital twins

Now that you have uploaded a model, you can use it to create digital twins, which are instances of a model, representing the entities within your business environment.

Add the following code to your application to create and initialize three digital twins based on the model you uploaded before:

```csharp
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
```

### Create relationships

Now that you have created a few digital twins, you can create relationships between them, to connect them into a *twin graph*, that represents your entire environment.

Add the following method to your project, either in its own class or as part of Program.cs:

```csharp
public async static Task CreateRelationshipAsync(DigitalTwinsClient client, string srcId, string targetId)
{
    var relationShip = new BasicRelationship
    {
        TargetId = targetId,
        Name = "contains"
    };

    try
    {
        var relId = $"{srcId}-contains->{targetId}";
        await client.CreateOrReplaceRelationshipAsync(srcId, relId, relationShip);
        Console.WriteLine("RelationShip created successfully!"); 
    }
    catch (RequestFailedException e)
    {
        Console.WriteLine($"Create relationship error: {e.Status}: {e.Message}");
    }
}
```

Call your newly created method to create relationships between your *sampleTwin-0* and *sampleTwin-1* and between *sampleTwin-0* and *sampleTwin-2*.

### List relationships

Finally, as a bonus challenge, write some code to list the relationships that you have created in the previous steps. You can find all relationships by calling the *GetRelationshipsAsync* method on your client object. You might want to wrap this API call in a method, similar as what you have done for creating relationships.

### Query your twin

It is relatively easy to query your twin graph programmatically to answer questions about your environment. The following query can be used to retrieve all digital twins in your ADT service instance: 

`Select * FROM digitaltwins`

To query the twin, you can call the QeuryAsync method on your client object and display its results using a JsonSerializer object.