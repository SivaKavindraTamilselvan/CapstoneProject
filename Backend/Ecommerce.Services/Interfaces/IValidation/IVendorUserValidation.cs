using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IVendorUserValidation
{
    public Task<VendorUser> ValidateInventoryVendorUserByUserId(int vendorUserId);
    public Task<VendorUser> ValidateOwnerVendorUserByUserId(int vendorUserId);
    public Task<VendorUser> ValidateProductVendorUserByUserId(int vendorUserId);
    public Task<VendorUser> ValidateVendorUserByUserId(int vendorUserId);
}