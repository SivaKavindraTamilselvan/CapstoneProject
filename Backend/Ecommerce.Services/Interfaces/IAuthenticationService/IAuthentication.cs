using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAuthentication
{
    public Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId);
    Task<User> RegisterUserWithoutPassword(RequestRegisterUserDTO requestRegisterUserDTO, int roleId);
    Task<ResponseSetPasswordDTO> SetPassword(RequestSetPasswordDTO requestSetPasswordDTO);
    public Task<ResponseRegisterUserDTO> ChangePassword(RequestChangePasswordDTO request, int userId);
    public Task<ResponseRegisterUserDTO> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO, int RoleId);
    public Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO);
    public Task<ResponseLoginUserDTO> Login(RequestLoginUserDTO requestLoginUserDTO);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
    public Task<ResponseRegisterVendorDTO> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO);
}