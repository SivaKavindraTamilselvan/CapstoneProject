using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IVendorValidation
{
    public Task<Vendor> ValidateVendor(int vendorId);
    public Task<Vendor> ValidateVendorIfApproved(int vendorId);
}