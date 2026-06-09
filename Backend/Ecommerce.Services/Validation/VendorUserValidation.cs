using System.Security.Authentication;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorUserValidation : IVendorUserValidation
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    public VendorUserValidation(IVendorUserRepsository vendorUserRepsository)
    {
        _vendorUserRepsository = vendorUserRepsository;
    }
    public async Task<VendorUser> ValidateVendorUserByUserId(int vendorUserId)
    {
        var vendorUser = await _vendorUserRepsository.GetVendorUserByUserId(vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        if(!vendorUser.IsActive)
        {
            throw new InvalidCredentialException("Vendor User Is Removed");
        }
        return vendorUser;
    }

}