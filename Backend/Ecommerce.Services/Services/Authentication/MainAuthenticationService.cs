using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    private readonly IUserRepsository _userRepsository;
    private readonly IAdminUserRepsository _adminRepository;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly ITokenService _tokenService;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMapper _mapper;
    public AuthenticationService(EcommerceContext ecommerceContext, IUserRepsository userRepsository, IAdminUserRepsository adminUserRepsository, IVendorRepsository vendorRepsository, IVendorUserRepsository vendorUserRepsository, ITokenService tokenService, ILogger<AuthenticationService> logger, IMapper mapper)
    {
        _ecommerceContext = ecommerceContext;
        _userRepsository = userRepsository;
        _adminRepository = adminUserRepsository;
        _vendorRepsository = vendorRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }

}