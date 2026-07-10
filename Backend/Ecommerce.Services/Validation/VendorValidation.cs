using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class VendorValidation :IVendorValidation
{
    private readonly ILogger<VendorValidation> _logger;
    private readonly IVendorRepsository _vendorRepsository;
    public VendorValidation(ILogger<VendorValidation> logger,IVendorRepsository vendorRepsository)
    {
        _logger = logger;
        _vendorRepsository = vendorRepsository;
    }
    public async Task<Vendor> ValidateVendor(int vendorId)
    {
        var vendor = await _vendorRepsository.Get(vendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Vendor review failed. VendorId {VendorId} not found", vendorId);
            throw new DataNotFoundException("Vendor ID Not Found");
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