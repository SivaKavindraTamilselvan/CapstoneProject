using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AddressRepsository : AbstractRepository<int, Address> ,IAddressRepsository
{
    public AddressRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<Address>> GetAddressByUserId(int userId)
    {
        return await _ecommerceContext.Address.Where(u=>u.UserId == userId).ToListAsync();
    }
    public async Task<List<Address>> GetActiveAddressByUserId(int userId)
    {
        return await _ecommerceContext.Address.Where(u=>u.UserId == userId && u.IsActive == true).ToListAsync();
    }
}