using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorValidation :IVendorValidation
{
    private readonly IVendorRepsository _vendorRepsository;
    public VendorValidation(IVendorRepsository vendorRepsository)
    {
        _vendorRepsository = vendorRepsository;
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