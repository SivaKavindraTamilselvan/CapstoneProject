using System.Security.Cryptography;
using System.Text;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyKeyRepsository _idempotencyKeyRepsository;

    public IdempotencyService(IIdempotencyKeyRepsository idempotencyKeyRepsository)
    {
        _idempotencyKeyRepsository = idempotencyKeyRepsository;
    }

    public async Task<(bool IsDuplicate, IdempotencyKey? Existing)> TryBeginOperation(
        string key, int userId, string requestBody)
    {
        var requestHash = ComputeHash(requestBody);
        var existing = await _idempotencyKeyRepsository.GetByKey(key);

        if (existing != null)
        {
            if (existing.UserId != userId || existing.RequestHash != requestHash)
            {
                throw new InvalidOperationException(
                    "This idempotency key is invalid for this request.");
            }
            return (true, existing);
        }

        var newEntry = new IdempotencyKey
        {
            Key = key,
            UserId = userId,
            RequestHash = requestHash,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IsCompleted = false
        };
        var created = await _idempotencyKeyRepsository.Create(newEntry);
        return (false, created);
    }

    public async Task CompleteOperation(int idempotencyKeyId, int statusCode, string responseBody)
    {
        await _idempotencyKeyRepsository.MarkCompleted(idempotencyKeyId, statusCode, responseBody);
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}