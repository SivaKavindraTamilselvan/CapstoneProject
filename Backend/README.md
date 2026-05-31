## Installation

- dotnet new sln -n Ecommerce (create solution file) (for single build)
- dotnet new webapi -n Ecommerce.API --use-controllers (create the App)
- dotnet new classlib -n Ecommerce.Models
- dotnet new classlib -n Ecommerce.Repositories
- dotnet new classlib -n Ecommerce.Services
- dotnet new classlib -n Ecommerce.Data

- dotnet sln add Ecommerce.API/Ecommerce.API.csproj
- dotnet sln add Ecommerce.Models/Ecommerce.Models.csproj
- dotnet sln add Ecommerce.Repositories/Ecommerce.Repositories.csproj
- dotnet sln add Ecommerce.Services/Ecommerce.Services.csproj
- dotnet sln add Ecommerce.Data/Ecommerce.Data.csproj

- dotnet add Ecommerce.Data reference Ecommerce.Models
- dotnet add Ecommerce.Repositories reference Ecommerce.Data
- dotnet add Ecommerce.Services reference Ecommerce.Repositories
- dotnet add Ecommerce.API reference Ecommerce.Services 

- dotnet add package Swashbuckle.AspNetCore

## Models Added

- The Model Design is Made
- Code First Approach
- Created the DBContext the main class which communicates with the EF Core
- Model Builder For each of the model table is added
- in Program.cs mention the context region that is configure the DBContext

- dotnet ef migrations add InitialCreate --project Ecommerce.Data --startup-project Ecommerce.API
- dotnet ef database update --project Ecommerce.Data --startup-project Ecommerce.API
- dotnet ef migrations remove --project Ecommerce.Data --startup-project Ecommerce.API
