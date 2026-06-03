using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    private readonly IProductRepsository _productRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductVariantAttributeRepsository _productVariantAttributeRepsository;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductValidation _productValidation;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IMapper _mapper;


    public VendorProductService(IMapper mapper, IProductRepsository productRepsository, IProductVariantRepsository productVariantRepsository, IProductImageRepsository productImageRepsository, IProductVariantAttributeRepsository productVariantAttributeRepsository,IVendorValidation vendorValidation,IProductValidation productValidation,IProductCategoryValidation productCategoryValidation)
    {
        _productRepsository = productRepsository;
        _productVariantRepsository = productVariantRepsository;
        _productImageRepsository = productImageRepsository;
        _productVariantAttributeRepsository = productVariantAttributeRepsository;
        _vendorValidation = vendorValidation;
        _productValidation = productValidation;
        _productCategoryValidation = productCategoryValidation;
        _mapper = mapper;
    }
}