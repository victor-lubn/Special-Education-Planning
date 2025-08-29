# Education View


## Getting Started
### Prerequisites
This must be installed globally on your computer:
	- Microsoft SQL Server Management Studio 18
	- Visual Studio 2019

## KOA installation
To be able to run the application, we must consume KOA Packages in Visual Studio:
First, open the web browser:
	- Open aiep Azure DevOps (https://dev.azure.com/aiep)
	- Select Projects/Trade Education Platform
	- In the menu on the left, select "Artifacts"
	- In the drop-down menu, on the right hand side of "+Create feed", select KOA
	- Select "Connect to feed"
	- Select "Visual Studio"
	- Copy to clipboard the source URL
	
And now, open Visual Studio:
	- Select "Tools", and then "Options"
    - Expand the "NuGet Package Manager" section, and then select "Package Sources"
	- Enter the feed's name and the source URL, and then select the green (+) sign to add a source
    - Select "OK"
	- Finally, right click SpecialEducationPlanning
.Api.Host project and select Manage NuGet Packages, and you need to set the Package Source to "All"

## SQL database Local configuration
### SQL database access

- Open Microsoft SQL Server Management Studio and connect to server:
	Server type: Database Engine
	Server name: (LocalDb)\MSSQLLocalDB
	Authentication: Windows Authentication
	And select "Connect"
	
### SQL database publication
- Open SpecialEducationPlanning
 solution with Visual Studio
- If SpecialEducationPlanning
.Database project is incompatible:
    Right click on SpecialEducationPlanning
.Database project and select "Reload project"
- Right click SpecialEducationPlanning
.Database project and select "Publish" and a new window "Publish Database" will be opened
- Regarding Target database connection, select "Edit"
- At the top of the window, select "Browse" and select Local/MSSQLLocalDB
- Select "OK"
- Write SpecialEducationPlanning
 on the "Database name"
- Select "Publish"

### SQL database population
To how to populate the database, please refer to the GlobalStaticData.sql fil, in the SpecialEducationPlanning
.Database project Scripts folder

## Running the application
- In Visual Studio, right click on SpecialEducationPlanning
.Api.Host project and select "Set as Startup Project"
- Select "Run"

## AzureSearch configuration
To set AzureSearch enabled or disabled, go to appsettings.json file of SpecialEducationPlanning
.Api.Host project
	- AzureSearch enabled
In the AzureSearch section, set "Enabled" : true, and "LefthandSearchEnabled": true
	- AzureSearch disabled
In the AzureSearch section, set "Enabled" : false

To build the Search indexes and run the custom indexer, we will need to run two APIs in Swagger:
	- EnsureAzureSearchCreated(bool deleteAndRecreate = true)
	- RunCustomIndexer()

##Hangfire
To run Hangfire jobs, go to appsettings.json file of SpecialEducationPlanning
.Api.Host project:
In the Hangfire section, set "Enabled" : true

To disable Hangfire jobs
In the Hangfire section, set "Enabled" : false

Sometimes, an error occurs when hangfire is enabled.
In the datacontext.cs file , in the foreach loop, in line 379:
We get the excepction: System.NullReferenceException: 'Object reference not set to an instance of an object.', in the line 381.
To fix this error, you can either disable Hangfire or, if you need Hangfire enabled, just keep F5'ing through until the app is working.
