using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminService
{
    public Task<ResponseGetAdminUserDTO> ActivateAdminUser(int adminUserId,int logedusedId);
    public Task<ResponseGetAdminUserDTO> DeactivateAdminUser(int adminUserId,int logedusedId);
    public Task<ResponseGetAdminUserDTO> GetAdminUserByUserId(int userId,int logedusedId);
    public Task<PagedResponse<ResponseGetAdminUserDTO>> GetAllAdminUser(RequestAdiminUserFilter request,int logedusedId);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
}