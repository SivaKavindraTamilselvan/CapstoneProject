using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class VendorRepsository : AbstractRepository<int, Vendor>, IVendorRepsository
{
    public VendorRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Vendor?> GetVendorByCompanyName(string companyname)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.VendorCompanyName == companyname);
    }
    public async Task<Vendor?> GetVendorByCompanyEmail(string Email)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.CompanyEmail == Email);
    }
    public async Task<Vendor?> GetVendorByCompanyGSTNumber(string gst)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.GSTNumber == gst);
    }
    public async Task<Vendor?> GetVendorByCompanyPhoneNumber(string phone)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(p => p.CompanyPhoneNumber == phone);
    }
    public async Task<List<Vendor>> GetVendors(int? statusId, int pageNumber, int pageSize)
    {
        var query = _ecommerceContext.Vendor.AsQueryable();
        if (statusId.HasValue)
        {
            query = query.Where(p => p.ApprovalStatusId == statusId.Value);
        }
        var vendor = await query.OrderByDescending(p=>p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return vendor;
    }
    public async Task<Vendor?> GetOwnerVendorUserByVendorId(int vendorId)
    {
        return await _ecommerceContext.Vendor.Include(v => v.VendorUsers.Where(vu => vu.VendorRoleId == 1)).ThenInclude(v => v.User).Where(v => v.VendorId == vendorId).FirstOrDefaultAsync();
    }

}