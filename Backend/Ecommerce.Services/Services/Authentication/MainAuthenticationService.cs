using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    private readonly ILogChanges _logChanges;
    private readonly IEmailService _emailService;
    private readonly IPasswordSetTokenRepsository _passwordSetTokenRepsository;
    private readonly IUserRepsository _userRepsository;
    private readonly IAdminUserRepsository _adminRepository;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly ITokenService _tokenService;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ICartRepsository _cartRepsository;
    private readonly IFavoriteRepsository _favoriteRepsository;
    private readonly IRegistrationValidation _registrationValidation;
    private readonly IUserValidation _userValidation;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMapper _mapper;
    public AuthenticationService(ILogChanges logChanges,IEmailService emailService,IPasswordSetTokenRepsository passwordSetTokenRepsository,IUserValidation userValidation,IRegistrationValidation registrationValidation,EcommerceContext ecommerceContext, IUserRepsository userRepsository, IAdminUserRepsository adminUserRepsository, IVendorRepsository vendorRepsository, IVendorUserRepsository vendorUserRepsository, ITokenService tokenService, ILogger<AuthenticationService> logger, IMapper mapper,ICartRepsository cartRepsository,IFavoriteRepsository favoriteRepsository)
    {
        _logChanges = logChanges;
        _emailService = emailService;
        _passwordSetTokenRepsository = passwordSetTokenRepsository;
        _userValidation = userValidation;
        _ecommerceContext = ecommerceContext;
        _userRepsository = userRepsository;
        _adminRepository = adminUserRepsository;
        _vendorRepsository = vendorRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _tokenService = tokenService;
        _favoriteRepsository = favoriteRepsository;
        _cartRepsository = cartRepsository;
        _registrationValidation = registrationValidation;
        _logger = logger;
        _mapper = mapper;
    }

}