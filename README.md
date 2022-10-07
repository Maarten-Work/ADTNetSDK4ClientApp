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

## Create a ADT sample graph using the Azure CLI

After executing the previous section, finish [this tutorial](https://learn.microsoft.com/en-us/azure/digital-twins/tutorial-command-line-cli) to create an ADT graph using the Azure CLI. 

> NOTE: The sample model files Room.json and Floor.json are already available in this repository (./SampleModelFiles).

