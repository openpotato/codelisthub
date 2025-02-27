![GitHub](https://img.shields.io/github/license/openpotato/codelisthub)

# CodeListHub

The service behind CodeListHub. Build with [.NET 9](https://dotnet.microsoft.com/).

## Technology stack

+ [PostgreSQL 17+](https://www.postgresql.org/) as database
+ [ASP.NET 9](https://dotnet.microsoft.com/apps/aspnet) as web framework
+ [Entity Framework Core 9](https://docs.microsoft.com/ef/) as ORM layer
+ [Swagger UI](https://swagger.io/tools/swagger-ui/) for OpenAPI based documentation

## Getting started 

The following instructions show you how to set up a development environment on your computer.

### Prerequisites

+ Set up a local PosgreSQL 17 (or higher) instance.
+ Clone or download the repository [CodeListHub.Data](https://github.com/openpotato/codelisthub.data).
+ Clone or download this repository.
+ Open the solution file `CodeListHub.sln` in Visual Studio 2022.

### Configure the CodeListHub CLI

+ Switch to the project `CodeListHub.CLI`.
+ Make a copy of the the `appsettings.json` file and name it `appsettings.Development.json`.
+ Exchange the content with the following JSON document and adjust the values to your needs. This configures the root folder for the csv data sources (the `src` folder in your local [OpenHolidaysApi.Data](https://github.com/openpotato/codelisthub.data) repository) and the database connection.
  
    ``` json
    "Sources": {
      "RootFolderName": "c:\\codelisthub.data\\src"
    },
    "Database": {
      "Server": "localhost",
      "Database": "CodeListHub",
      "Username": "postgres",
      "Password": "qwertz"
    }
    ```

### Create and populate the database

+ Build the `CodeListHub.CLI` project. 
+ Run the `CodeListHub.CLI` project with parameter `initdb --import`. This will create and populate the PostgreSQL database.

### Configure the CodeListHub WebService

+ Switch to the  `CodeListHub.WebService`. 
+ Make a copy of the the `appsettings.json` file and name it `appsettings.Development.json`.
+ Exchange the content with the following JSON document and adjust the values to your needs. This configures the database connection.

    ``` json
    "Database": {
      "Server": "localhost",
      "Database": "CodeListHub",
      "Username": "postgres",
      "Password": "qwertz"
    }
    ```

### Build and test the API

+ Build the `CodeListHub.WebService` project.
+ Run the `CodeListHub.WebService` project and play with the Swagger UI.

## Can I help?

Yes, that would be much appreciated. The best way to help is to post a response via the Issue Tracker and/or submit a Pull Request.
