using Ecommerce.DTOs;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminService : IAdminService
{
    private readonly IAuthentication _authentication;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    public AdminService(IAuthentication authentication,IVendorRepsository vendorRepsository,IAdminUserRepsository adminUserRepsository)
    {
        _authentication = authentication;
        _vendorRepsository = vendorRepsository;
        _adminUserRepsository = adminUserRepsository;
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO,int adminUserId)
    {
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO,adminUserId);
        return result;
    }
}