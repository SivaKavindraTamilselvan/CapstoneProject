namespace Ecommerce.Repositories.Interfaces;

public interface IRepository<K,T> where T : class
{
    public Task<T?> Create(T item);
    public Task<List<T>> GetAll();
    public Task<T?> Get(K key);
    public Task<T?> Update(K key,T item);
    public Task<T?> Delete(K key);
}