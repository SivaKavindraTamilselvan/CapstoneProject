using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAuthentication
{
    public Task<ResponseRegisterUserDTO> ChangePassword(RequestChangePasswordDTO request, int userId);
    public Task<ResponseRegisterUserDTO> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO, int RoleId);
    public Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO);
    public Task<ResponseLoginUserDTO> Login(RequestLoginUserDTO requestLoginUserDTO);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
    public Task<ResponseRegisterVendorDTO> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO);
}