using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminService
{
    public Task<ResponseGetAdminUserDTO> ActivateAdminUser(int adminUserId);
    public Task<ResponseGetAdminUserDTO> DeactivateAdminUser(int adminUserId);
    public Task<ResponseGetAdminUserDTO> GetAdminUserByUserId(int userId);
    public Task<PagedResponse<ResponseGetAdminUserDTO>> GetAllAdminUser(RequestAdiminUserFilter request);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
}