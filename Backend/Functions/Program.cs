using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Ecommerce.API.Services;
using Ecommerce.Data;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];

if (!string.IsNullOrWhiteSpace(keyVaultName))
{
    var vaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(vaultUri, credential);

    builder.Configuration["ConnectionStrings:Default"] =
        secretClient.GetSecret("PostgresConnectionString").Value.Value;

    builder.Configuration["BlobStorage:ConnectionString"] =
        secretClient.GetSecret("BlobStorageConnectionString").Value.Value;

    builder.Configuration["Jwt:Key"] =
        secretClient.GetSecret("JwtKey").Value.Value;

    builder.Configuration["EmailSettings:SmtpHost"] =
        secretClient.GetSecret("EmailSmtpHost").Value.Value;

    builder.Configuration["EmailSettings:SmtpPort"] =
        secretClient.GetSecret("EmailSmtpPort").Value.Value;

    builder.Configuration["EmailSettings:SenderName"] =
        secretClient.GetSecret("EmailSenderName").Value.Value;

    builder.Configuration["EmailSettings:SenderEmail"] =
        secretClient.GetSecret("EmailSenderEmail").Value.Value;

    builder.Configuration["EmailSettings:Username"] =
        secretClient.GetSecret("EmailUsername").Value.Value;

    builder.Configuration["EmailSettings:Password"] =
        secretClient.GetSecret("EmailPassword").Value.Value;

    builder.Configuration["EmailSettings:UseSsl"] =
        secretClient.GetSecret("EmailUseSsl").Value.Value;
}

// Register DbContext
builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Register services/repositories needed by CheckAbandonedCarts
builder.Services.AddScoped<INotificationRepsository, NotificationRepsository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Build().Run();