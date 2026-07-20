using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly ILogChanges _logChanges;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogger<AdminProductCategoryService> _logger;
    private readonly INotificationService _notificationService;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    public AdminProductCategoryService(ILogChanges logChanges,EcommerceContext ecommerceContext,ILogger<AdminProductCategoryService> logger,INotificationService notificationService,IVendorRepsository vendorRepsository,IAdminUserValidation adminUserValidation,IMapper mapper, IProductCategoryRepsository productCategoryRepsository, IProductSubCategoryRepsository productSubCategoryRepsository, IProductCategoryValidation productCategoryValidation)
    {
        _logger = logger;
        _notificationService = notificationService;
        _vendorRepsository = vendorRepsository;
        _productCategoryRepsository = productCategoryRepsository;
        _adminUserValidation = adminUserValidation;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _productCategoryValidation = productCategoryValidation;
        _mapper = mapper;
        _ecommerceContext = ecommerceContext;
        _logChanges = logChanges;
    }
}