using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IVendorUserValidation
{
    public Task<VendorUser> ValidateVendorUser(int vendorUserId);
}