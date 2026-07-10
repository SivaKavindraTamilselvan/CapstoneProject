using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly ILogger<VendorProductVariantService> _logger;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductAttributeValidation _productAttributeValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductVariantAttributeRepsository _productVariantAttributeRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IVendorUserRepsository _vendorUserRepsository;

    public VendorProductVariantService(IVendorUserRepsository vendorUserRepsository,INotificationService notificationService,IAdminUserRepsository adminUserRepsository,ILogger<VendorProductVariantService> logger,IVendorValidation vendorValidation,IProductAttributeValidation productAttributeValidation,IMapper mapper, IProductRepsository productRepsository, IProductVariantRepsository productVariantRepsository, IProductImageRepsository productImageRepsository, IProductVariantAttributeRepsository productVariantAttributeRepsository,IProductValidation productValidation,IProductCategoryValidation productCategoryValidation,IVendorUserValidation vendorUserValidation)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _notificationService = notificationService;
        _adminUserRepsository = adminUserRepsository;
        _vendorValidation = vendorValidation;
        _productAttributeValidation = productAttributeValidation;
        _productVariantRepsository = productVariantRepsository;
        _productVariantAttributeRepsository = productVariantAttributeRepsository;
        _productValidation = productValidation;
        _vendorUserValidation = vendorUserValidation;
        _mapper = mapper;
        _logger = logger;
    }
}