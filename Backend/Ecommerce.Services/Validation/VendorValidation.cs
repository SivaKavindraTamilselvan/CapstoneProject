using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorValidation :IVendorValidation
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IInventoryRepsository _inventoryRepsository;
    public VendorValidation(IVendorUserRepsository vendorUserRepsository,IVendorRepsository vendorRepsository,IInventoryRepsository inventoryRepsository)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _vendorRepsository = vendorRepsository;
        _inventoryRepsository = inventoryRepsository;
    }
    public async Task<VendorUser> ValidateVendorUser(int vendorUserId)
    {
        var vendorUser = await _vendorUserRepsository.GetVendorUserByUserId(vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        return vendorUser;
    }

    public async Task<Vendor> ValidateVendor(int vendorId)
    {
        var vendor = await _vendorRepsository.Get(vendorId);
        if (vendor == null)
        {
            throw new DataNotFoundException("Vendor Not Found");
        }
        return vendor;
    }

    public async Task<Vendor> ValidateVendorIfApproved(int vendorId)
    {
        var vendor = await ValidateVendor(vendorId);
        if(vendor.ApprovalStatusId !=2)
        {
            throw new DataApprovalStatusException("The Vendor Is Not Approved Yet");
        }
        return vendor;
    }

    
}