using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly ILogger<AdminService> _logger;
    private readonly IAuthentication _authentication;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IUserRepsository _userRepsository;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IMapper _mapper;
    public AdminService(IAdminUserValidation adminUserValidation,EcommerceContext ecommerceContext, IUserRepsository userRepsository, IAuthentication authentication, IMapper mapper, ILogger<AdminService> logger, IAdminUserRepsository adminUserRepsository)
    {
        _adminUserValidation = adminUserValidation;
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _adminUserRepsository = adminUserRepsository;
        _userRepsository = userRepsository;
        _mapper = mapper;
        _logger = logger;
    }
}