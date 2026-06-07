using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    private readonly IProductRepsository _productRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IApprovalHistoryRepsository _approvalHistoryRepsository;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    public AdminProductService(IAdminUserValidation adminUserValidation,IMapper mapper, IProductRepsository productRepsository, IProductCategoryRepsository productCategoryRepsository, IProductSubCategoryRepsository productSubCategoryRepsository, IAttributeRepsository attributeRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository, IProductValidation productValidation, IVendorValidation vendorValidation, IApprovalHistoryRepsository approvalHistoryRepsository, IProductCategoryValidation productCategoryValidation)
    {
        _productRepsository = productRepsository;
        _approvalHistoryRepsository = approvalHistoryRepsository;
        _productValidation = productValidation;
        _adminUserValidation = adminUserValidation;
        _mapper = mapper;
    }
}