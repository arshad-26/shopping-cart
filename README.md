# ShoppingCart
This is a simple shopping cart website created with functionalities like adding categories and items, adding items to a cart and placing orders with User and Admin logins and rating orders. Users can register themselves to this application.

## Technologies Used
* Backend - ASP.NET Core Web API
* Frontend - Blazor WebAssembly
* Data Access - SQL Server with EF Core
* Authentication/Authorization - ASP.Net Core Identity with JWT Authentication model
* UI Addons - MudBlazor which is a UI Component Library for Blazor
* Elasticsearch and Kibana - Used along with Serilog for logging. Able to be configured as docker containers and attached the yaml file as well.

### Steps to integrate
* In the **appsettings.{environment}.json** file in the **ShoppingCartAPI** project, Change the connection string to point to a database of your choice.
* In the above file, Give a value for **ValidIssuer**, **ValidAudience** and the **secret** to be used for JWT authentication.
* run **update-database** command in NuGet Package Manager Console or the .NET CLI to get the desired DB schema. The migration files are already included in the project.
