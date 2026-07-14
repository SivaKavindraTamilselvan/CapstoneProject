// Services/Interfaces/IBlobStorageService.cs
namespace Ecommerce.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteImageAsync(string blobUrl);
    }
}