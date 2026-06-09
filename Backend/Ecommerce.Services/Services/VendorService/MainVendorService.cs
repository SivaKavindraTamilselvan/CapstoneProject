using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorService : IVendorService
{
    private readonly IAuthentication _authentication;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductRepsository _productRepsository;
    private readonly IApprovalHistoryRepsository _approvalHistoryRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IMapper _mapper;


    public VendorService(IProductVariantRepsository productVariantRepsository,IApprovalHistoryRepsository approvalHistoryRepsository,IProductRepsository productRepsository, IVendorUserValidation vendorUserValidation,IProductValidation productValidation,IMapper mapper,EcommerceContext ecommerceContext, IAuthentication authentication,IVendorUserRepsository vendorUserRepsository)
    {
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _vendorUserRepsository = vendorUserRepsository;
        _productValidation = productValidation;
        _vendorUserValidation = vendorUserValidation;
        _productRepsository = productRepsository;
        _approvalHistoryRepsository = approvalHistoryRepsository;
        _productVariantRepsository = productVariantRepsository;
        _mapper = mapper;
    }
}