using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class RegsitrationValidation : IRegistrationValidation
{
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IUserRepsository _userRepsository;
    public RegsitrationValidation(IVendorRepsository vendorRepsository,IUserRepsository userRepsository)
    {
        _vendorRepsository = vendorRepsository;
        _userRepsository = userRepsository;
    }
    public async Task ValidateVendorDetails(RequestRegisterVendorDTO requestRegisterVendorDTO)
    {
        var vendorByCompanyName = await _vendorRepsository.GetVendorByCompanyName(requestRegisterVendorDTO.VendorCompanyName);
        if (vendorByCompanyName != null)
        {
            throw new DataAlreadyRegisteredException("Vendor with company name already registered");
        }
        var vendorByCompanyEmail = await _vendorRepsository.GetVendorByCompanyEmail(requestRegisterVendorDTO.CompanyEmail);
        if (vendorByCompanyEmail != null)
        {
            throw new DataAlreadyRegisteredException("Vendor with company email already registered");
        }
        var vendorByPhone = await _vendorRepsository.GetVendorByCompanyPhoneNumber(requestRegisterVendorDTO.CompanyPhoneNumber);
        if (vendorByPhone != null)
        {
            throw new DataAlreadyRegisteredException("Vendor with company phone already registered");
        }
        var vendorByGST = await _vendorRepsository.GetVendorByCompanyGSTNumber(requestRegisterVendorDTO.GSTNumber);
        if (vendorByGST != null)
        {
            throw new DataAlreadyRegisteredException("Vendor with company gst already registered");
        }
    }
    public async Task ValidateUserDetails(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        var existingEmailUser = await _userRepsository.GetUserByEmail(requestRegisterUserDTO.Email);
        if (existingEmailUser != null)
        {
            throw new DataAlreadyRegisteredException("User Already Registered With The Email.");
        }
        var existingPhoneNumberUser = await _userRepsository.GetUserByPhoneNumber(requestRegisterUserDTO.PhoneNumber);
        if (existingPhoneNumberUser != null)
        {
            throw new DataAlreadyRegisteredException("User Already Registered With The PhoneNumber.");
        }
    }
}