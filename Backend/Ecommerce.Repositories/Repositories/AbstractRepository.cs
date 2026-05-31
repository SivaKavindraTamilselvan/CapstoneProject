using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AbstractRepository<K,T> : IRepository<K,T> where T : class
{
    protected readonly EcommerceContext _ecommerceContext;
    public AbstractRepository(EcommerceContext ecommerceContext)
    {
        _ecommerceContext = ecommerceContext;
    }
    public async Task<T?> Create(T item)
    {
        _ecommerceContext.Add(item);
        await _ecommerceContext.SaveChangesAsync();
        return item;
    }
    public async Task<T?> Get(K key)
    {
        var result = await _ecommerceContext.FindAsync<T>(key);
        return result;
    } 
    public async Task<List<T>> GetAll()
    {
        var result = await _ecommerceContext.Set<T>().ToListAsync();
        return result;
    }
    public async Task<T?> Update(K key,T item)
    {
        var result = await Get(key);
        if(result == null)
        {
            throw new Exception("No such item is found");
        }
        _ecommerceContext.Update(item);
        await _ecommerceContext.SaveChangesAsync();
        return item;
    }

    public async Task<T?> Delete(K key)
    {
        var result = await Get(key);
        if(result == null)
        {
            throw new Exception("No such item is found");
        }
        _ecommerceContext.Remove(result);
        await _ecommerceContext.SaveChangesAsync();
        return result;
    }
}