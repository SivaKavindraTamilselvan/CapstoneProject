using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IProductValidation _productValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<VendorProductService> _logger;
    private readonly INotificationService _notificationService;


    public VendorProductService(IAdminUserRepsository adminUserRepsository,IVendorUserRepsository vendorUserRepsository,INotificationService notificationService,ILogger<VendorProductService> logger,IProductVariantRepsository productVariantRepsository,IProductValidation productValidation,IVendorUserValidation vendorUserValidation,IMapper mapper, IProductRepsository productRepsository, IVendorValidation vendorValidation,IProductCategoryValidation productCategoryValidation)
    {
        _adminUserRepsository = adminUserRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _notificationService = notificationService;
        _productVariantRepsository = productVariantRepsository;
        _vendorUserValidation = vendorUserValidation;
        _productRepsository = productRepsository;
        _vendorValidation = vendorValidation;
        _productCategoryValidation = productCategoryValidation;
        _productValidation = productValidation;
        _mapper = mapper;
        _logger = logger;
    }
}