using Ecommerce.Models;

public interface IIdempotencyService
{
    Task<(bool IsDuplicate, IdempotencyKey? Existing)> TryBeginOperation(string key, int userId, string requestBody);
    Task CompleteOperation(int idempotencyKeyId, int statusCode, string responseBody);
}