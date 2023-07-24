# ShoppingCart
This is a simple shopping cart website created with functionalities like adding categories and items, adding items to a cart and placing orders with User and Admin logins. Users can register themselves to this application.

## Technologies Used
The backend involves a Web API project in ASP.NET Core. The frontend is written in Blazor WebAssembly which communicates with the API following a SPA architecture. The database used is a SQL Server DB and data manipulation and fetching logic is implemented in EF Core. Authentication and Authorization is done through ASP.Net Core Identity and a JWT based authentication model. All the UI Component logic is written in a library called MudBlazor which integrates seamlessly with Blazor.

### Steps to integrate
In the appsettings.{environment}.json file in the ShoppingCartAPI project, Change the connection string to point to a database of your choice.
Give a value for ValidIssuer, ValidAudience and the secret key to be used for JWT authentication.
run update-database command in NuGet Package Manager Console or the .NET CLI to get the desired DB schema. The migration files are already included in the project.
