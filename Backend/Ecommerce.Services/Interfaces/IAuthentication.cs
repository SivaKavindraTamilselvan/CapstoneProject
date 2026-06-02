using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAuthentication
{
    public Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO);
    public Task<ResponseLoginUserDTO> Login(RequestLoginUserDTO requestLoginUserDTO);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO,int adminUserId);
    public Task<ResponseRegisterVendorDTO> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO);
}