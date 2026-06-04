using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAddressRepsository : IRepository<int, Address>
{
    public Task<List<Address>> GetAddressByUserId(int userId);
}