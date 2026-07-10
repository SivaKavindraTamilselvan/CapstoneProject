using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IRegistrationValidation
{
    public Task ValidateUserDetails(RequestRegisterUserDTO requestRegisterUserDTO);
    public Task ValidateVendorDetails(RequestRegisterVendorDTO requestRegisterVendorDTO);
}