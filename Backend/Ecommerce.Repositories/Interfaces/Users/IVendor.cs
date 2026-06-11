using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IVendorRepsository : IRepository<int,Vendor>
{
    public Task<Vendor?> GetVendorByCompanyName(string companyname);
    public Task<Vendor?> GetVendorByCompanyEmail(string Email);
    public Task<Vendor?> GetVendorByCompanyGSTNumber(string gst);
    public Task<Vendor?> GetVendorByCompanyPhoneNumber(string phone);
    public Task<Vendor?> GetOwnerVendorUserByVendorId(int vendorId);
    public Task<(List<Vendor> items,int totalCount)> GetVendorsForAdmin(RequestAdminVendorFilter request);
}