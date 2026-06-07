using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IReturnRepsository : IRepository<int,Return>
{
    public Task<Return?> GetTheReturnUserByReturnId(int returnId);
    public Task<Return?> GetTheReturnInventoryByReturnId(int returnId);
    public Task<Return?> GetTheReturnProductByReturnId(int returnId);
}