using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    private readonly ILogger<AdminVendorService> _logger;
    private readonly IVendorValidation _vendorValidation;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    public AdminVendorService(IVendorRepsository vendorRepsository,IAdminUserValidation adminUserValidation,IVendorValidation vendorValidation,ILogger<AdminVendorService> logger,INotificationService notificationService,IMapper mapper,IVendorUserRepsository vendorUserRepsository,IAdminUserRepsository adminUserRepsository)
    {
        _adminUserValidation = adminUserValidation;
        _vendorValidation = vendorValidation;
        _vendorRepsository = vendorRepsository;
        _notificationService = notificationService;
        _vendorUserRepsository = vendorUserRepsository;
        _adminUserRepsository = adminUserRepsository;
        _mapper = mapper;
        _logger = logger;
    }
}