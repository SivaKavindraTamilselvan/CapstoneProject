using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly EcommerceContext _ecommerceContext;
    private readonly INotificationService _notificationService;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IProductAttributeValidation _productAttributeValidation;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminProductAttributeService> _logger;
    private readonly ILogChanges _logChanges;
    public AdminProductAttributeService(IVendorUserRepsository vendorUserRepsository,ILogChanges logChanges,EcommerceContext ecommerceContext,INotificationService notificationService,IVendorRepsository vendorRepsository,ILogger<AdminProductAttributeService> logger,IMapper mapper,IAttributeRepsository attributeRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IProductAttributeValidation productAttributeValidation,IAdminUserValidation adminUserValidation)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _logChanges = logChanges;
        _notificationService = notificationService;
        _vendorRepsository = vendorRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _productAttributeValidation = productAttributeValidation;
        _adminUserValidation = adminUserValidation;
        _ecommerceContext = ecommerceContext;
        _logger = logger;
        _mapper = mapper;
    }
}