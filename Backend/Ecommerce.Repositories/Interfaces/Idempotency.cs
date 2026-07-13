using Ecommerce.Models;

public interface IIdempotencyKeyRepsository
{
    Task<IdempotencyKey?> GetByKey(string key);
    Task<IdempotencyKey> Create(IdempotencyKey idempotencyKey);
    Task<IdempotencyKey?> MarkCompleted(int id, int statusCode, string responseBody);
}