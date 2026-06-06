using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    private readonly IProductAttributeValidation _productAttributeValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductVariantAttributeRepsository _productVariantAttributeRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;


    public VendorProductVariantService(IProductAttributeValidation productAttributeValidation,IMapper mapper, IProductRepsository productRepsository, IProductVariantRepsository productVariantRepsository, IProductImageRepsository productImageRepsository, IProductVariantAttributeRepsository productVariantAttributeRepsository,IVendorValidation vendorValidation,IProductValidation productValidation,IProductCategoryValidation productCategoryValidation,IVendorUserValidation vendorUserValidation)
    {
        _productAttributeValidation = productAttributeValidation;
        _productVariantRepsository = productVariantRepsository;
        _productVariantAttributeRepsository = productVariantAttributeRepsository;
        _productValidation = productValidation;
        _vendorUserValidation = vendorUserValidation;
        _mapper = mapper;
    }
}