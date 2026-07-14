// Services/BlobStorageService.cs
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _logger = logger;
            var connectionString = configuration["BlobStorage:ConnectionString"];
            var containerName = "product-images"; // matches the container you already provisioned via Bicep

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = _containerClient.GetBlobClient(uniqueFileName);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            _logger.LogInformation("Uploaded image {FileName} to blob storage as {UniqueFileName}", fileName, uniqueFileName);

            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl)) return;

            var uri = new Uri(blobUrl);
            var blobName = Path.GetFileName(uri.LocalPath);
            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
            _logger.LogInformation("Deleted blob {BlobName}", blobName);
        }
    }
}