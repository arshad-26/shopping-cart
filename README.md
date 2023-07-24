# ShoppingCart
This is a simple shopping cart website created with functionalities like adding categories and items, adding items to a cart and placing orders with User and Admin logins. Users can register themselves to this application.

## Technologies Used
* Backend - ASP.NET Core Web API
* Frontend - Blazor WebAssembly
* Data Access - SQL Server with EF Core
* Authentication/Authorization - ASP.Net Core Identity with JWT Authentication model
* UI Addons - MudBlazor which is a UI Component Library for Blazor

### Steps to integrate
* In the **appsettings.{environment}.json** file in the **ShoppingCartAPI** project, Change the connection string to point to a database of your choice.
* Give a value for ValidIssuer, ValidAudience and the secret key to be used for JWT authentication.
* run **update-database** command in NuGet Package Manager Console or the .NET CLI to get the desired DB schema. The migration files are already included in the project.
