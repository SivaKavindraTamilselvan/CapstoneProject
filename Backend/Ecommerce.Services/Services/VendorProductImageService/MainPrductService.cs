using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    private readonly ILogChanges _logChanges;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IVendorValidation _vendorValidation;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<VendorProductImageService> _logger;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;


    public VendorProductImageService(ILogChanges logChanges,EcommerceContext ecommerceContext,IVendorValidation vendorValidation,IAdminUserRepsository adminUserRepsository,INotificationService notificationService,ILogger<VendorProductImageService> logger,IVendorUserValidation vendorUserValidation,IMapper mapper,IProductImageRepsository productImageRepsository,IProductValidation productValidation)
    {
        _logChanges = logChanges;
        _ecommerceContext = ecommerceContext;
        _vendorValidation = vendorValidation;
        _adminUserRepsository = adminUserRepsository;
        _notificationService = notificationService;
        _vendorUserValidation = vendorUserValidation;
        _productImageRepsository = productImageRepsository;     
        _productValidation = productValidation;
        _mapper = mapper;
        _logger = logger;
    }
}