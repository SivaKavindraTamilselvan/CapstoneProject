using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AddressRepsository : AbstractRepository<int, Address>, IAddressRepsository
{
    public AddressRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<List<Address>> GetAllAddressByUserId(int userId)
    {
        return await _ecommerceContext.Address.Where(u=>u.UserId == userId).ToListAsync();
    }
    public async Task<List<Address>> GetAddressByUserId(int userId, bool? status, int pageNumber, int pageSize)
    {
        IQueryable<Address> query = _ecommerceContext.Address.Where(u => u.UserId == userId);
        if (status.HasValue)
        {
            query = query.Where(a => a.IsActive == status);
        }
        var Address = await query.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return Address;
    }
    public async Task<List<Address>> GetActiveAddressByUserId(int userId, int pageNumber, int pageSize)
    {
        return await _ecommerceContext.Address.Where(u => u.UserId == userId && u.IsActive == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}