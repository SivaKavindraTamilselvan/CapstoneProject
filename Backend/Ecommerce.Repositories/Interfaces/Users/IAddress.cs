using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAddressRepsository : IRepository<int, Address>
{
    public Task<List<Address>> GetAllAddressByUserId(int userId);
    public Task<List<Address>> GetActiveAddressByUserId(int userId, int pageNumber, int pageSize);
    public Task<List<Address>> GetAddressByUserId(int userId,bool? status,int pageNumber,int pageSize);
}