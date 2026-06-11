using Ecommerce.Data;
using Ecommerce.DTOs;
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
        return await _ecommerceContext.Address.Where(u => u.UserId == userId).ToListAsync();
    }

    // vendor inventory address based on the vendor owner id
    public async Task<(List<Address> Items, int totalCount)> GetAddressByVendorOwnerId(AddressRequestFilter requestFilter, int ownerVendorUserId)
    {
        IQueryable<Address> query = _ecommerceContext.Address.Where(u => u.UserId == ownerVendorUserId);
        if (!string.IsNullOrWhiteSpace(requestFilter.City))
        {
            query = query.Where(a => a.City.ToLower() == requestFilter.City.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(requestFilter.ContactPhoneNumber))
        {
            query = query.Where(a => a.ContactPhoneNumber.ToLower() == requestFilter.ContactPhoneNumber.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(requestFilter.Country))
        {
            query = query.Where(a => a.Country.ToLower() == requestFilter.Country.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(requestFilter.PinCode))
        {
            query = query.Where(a => a.PinCode == requestFilter.PinCode);
        }
        if (!string.IsNullOrWhiteSpace(requestFilter.State))
        {
            query = query.Where(a => a.State.ToLower() == requestFilter.State.ToLower());
        }
        if (requestFilter.IsActive.HasValue)
        {
            query = query.Where(a => a.IsActive == requestFilter.IsActive.Value);
        }
        int totalCount = await query.CountAsync();
        var Address = await query.OrderByDescending(c => c.CreatedAt).Skip((requestFilter.PageNumber - 1) * requestFilter.PageSize).Take(requestFilter.PageSize).ToListAsync();
        return (Address, totalCount);
    }
    public async Task<List<Address>> GetAllActiveUserAddress(int userId)
    {
        return await _ecommerceContext.Address.Where(u => u.UserId == userId && u.IsActive == true).ToListAsync();
    }
}