using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminService : IAdminService
{
    private readonly IAuthentication _authentication;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IMapper _mapper;
    public AdminService(IAuthentication authentication,IMapper mapper,IVendorRepsository vendorRepsository,IAdminUserRepsository adminUserRepsository,IProductRepsository productRepsository,IVendorUserRepsository vendorUserRepsository)
    {
        _authentication = authentication;
        _vendorRepsository = vendorRepsository;
        _adminUserRepsository = adminUserRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _productRepsository = productRepsository;
        _mapper = mapper;
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO,int adminUserId)
    {
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO,adminUserId);
        return result;
    }
}