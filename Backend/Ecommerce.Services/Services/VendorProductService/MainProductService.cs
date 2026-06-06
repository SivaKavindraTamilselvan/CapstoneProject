using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    private readonly IProductRepsository _productRepsository;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;


    public VendorProductService(IVendorUserValidation vendorUserValidation,IMapper mapper, IProductRepsository productRepsository, IVendorValidation vendorValidation,IProductCategoryValidation productCategoryValidation)
    {
        _vendorUserValidation = vendorUserValidation;
        _productRepsository = productRepsository;
        _vendorValidation = vendorValidation;
        _productCategoryValidation = productCategoryValidation;
        _mapper = mapper;
    }
}