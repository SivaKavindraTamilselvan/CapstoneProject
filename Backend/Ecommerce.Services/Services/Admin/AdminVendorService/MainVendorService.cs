using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminVendorService : IAdminVendorService
{
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IMapper _mapper;
    public AdminVendorService(IMapper mapper,IVendorRepsository vendorRepsository,IAdminUserRepsository adminUserRepsository)
    {
        _vendorRepsository = vendorRepsository;
        _adminUserRepsository = adminUserRepsository;
        _mapper = mapper;
    }
}