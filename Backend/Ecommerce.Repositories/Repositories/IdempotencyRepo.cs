using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

public class IdempotencyKeyRepsository : IIdempotencyKeyRepsository
{
    private readonly EcommerceContext _ecommerceContext;

    public IdempotencyKeyRepsository(EcommerceContext ecommerceContext)
    {
        _ecommerceContext = ecommerceContext;
    }

    public async Task<IdempotencyKey?> GetByKey(string key)
    {
        return await _ecommerceContext.IdempotencyKeys
            .FirstOrDefaultAsync(i => i.Key == key);
    }

    public async Task<IdempotencyKey> Create(IdempotencyKey idempotencyKey)
    {
        _ecommerceContext.IdempotencyKeys.Add(idempotencyKey);
        await _ecommerceContext.SaveChangesAsync();
        return idempotencyKey;
    }

    public async Task<IdempotencyKey?> MarkCompleted(int id, int statusCode, string responseBody)
    {
        var entity = await _ecommerceContext.IdempotencyKeys
            .FirstOrDefaultAsync(i => i.IdempotencyKeyId == id);
        if (entity == null) return null;

        entity.IsCompleted = true;
        entity.ResponseStatusCode = statusCode;
        entity.ResponseBody = responseBody;
        await _ecommerceContext.SaveChangesAsync();
        return entity;
    }
}