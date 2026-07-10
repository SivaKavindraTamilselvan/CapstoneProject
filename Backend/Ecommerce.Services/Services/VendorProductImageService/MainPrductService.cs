using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductImageService : IVendorProductImageService
{
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<VendorProductImageService> _logger;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;


    public VendorProductImageService(IAdminUserRepsository adminUserRepsository,INotificationService notificationService,ILogger<VendorProductImageService> logger,IVendorUserValidation vendorUserValidation,IMapper mapper,IProductImageRepsository productImageRepsository,IProductValidation productValidation)
    {
        _adminUserRepsository = adminUserRepsository;
        _notificationService = notificationService;
        _vendorUserValidation = vendorUserValidation;
        _productImageRepsository = productImageRepsository;     
        _productValidation = productValidation;
        _mapper = mapper;
        _logger = logger;
    }
}