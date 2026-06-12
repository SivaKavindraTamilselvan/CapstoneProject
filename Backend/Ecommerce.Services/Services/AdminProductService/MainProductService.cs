using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IApprovalHistoryRepsository _approvalHistoryRepsository;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminProductService> _logger;
    public AdminProductService(IVendorUserRepsository vendorUserRepsository,ILogger<AdminProductService> logger,INotificationService notificationService,IProductVariantRepsository productVariantRepsository,IAdminUserValidation adminUserValidation,IMapper mapper, IProductRepsository productRepsository, IProductCategoryRepsository productCategoryRepsository, IProductSubCategoryRepsository productSubCategoryRepsository, IAttributeRepsository attributeRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository, IProductValidation productValidation, IVendorValidation vendorValidation, IApprovalHistoryRepsository approvalHistoryRepsository, IProductCategoryValidation productCategoryValidation)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _logger = logger;
        _notificationService = notificationService;
        _productVariantRepsository = productVariantRepsository;
        _productRepsository = productRepsository;
        _approvalHistoryRepsository = approvalHistoryRepsository;
        _productValidation = productValidation;
        _adminUserValidation = adminUserValidation;
        _mapper = mapper;
    }
}