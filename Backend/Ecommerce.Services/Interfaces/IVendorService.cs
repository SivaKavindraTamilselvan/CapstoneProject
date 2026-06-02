using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorService
{
    public Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO,int vendorUserId);
}