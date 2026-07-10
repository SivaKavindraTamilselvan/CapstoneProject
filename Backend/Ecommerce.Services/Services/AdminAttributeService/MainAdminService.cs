using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    private readonly INotificationService _notificationService;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IProductAttributeValidation _productAttributeValidation;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminProductAttributeService> _logger;
    public AdminProductAttributeService(INotificationService notificationService,IVendorRepsository vendorRepsository,ILogger<AdminProductAttributeService> logger,IMapper mapper,IAttributeRepsository attributeRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IProductAttributeValidation productAttributeValidation,IAdminUserValidation adminUserValidation)
    {
        _notificationService = notificationService;
        _vendorRepsository = vendorRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _productAttributeValidation = productAttributeValidation;
        _adminUserValidation = adminUserValidation;
        _logger = logger;
        _mapper = mapper;
    }
}