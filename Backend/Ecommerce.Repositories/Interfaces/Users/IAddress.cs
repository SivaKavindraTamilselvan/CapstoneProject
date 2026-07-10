using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAddressRepsository : IRepository<int, Address>
{

    public Task<List<Address>> GetAllAddressByUserId(int userId);
    public Task<List<Address>> GetAllActiveUserAddress(int userId);
    public Task<(List<Address> Items, int totalCount)> GetAddressByVendorOwnerId(AddressRequestFilter requestFilter, int ownerVendorUserId);
}