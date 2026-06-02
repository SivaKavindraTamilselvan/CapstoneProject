using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class ProductService : IProductService
{
    private readonly IAuthentication _authentication;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IProductRepsository _productRepsository;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IVendorRepsository _vendorRepository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IMapper _mapper;
    public ProductService(IMapper mapper, EcommerceContext ecommerceContext, IAuthentication authentication, IProductRepsository productRepsository,IVendorUserRepsository vendorUserRepsository,IVendorRepsository vendorRepsository,IProductImageRepsository productImageRepsository,IProductVariantRepsository productVariantRepsository)
    {
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _productRepsository = productRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _vendorRepository = vendorRepsository;
        _productImageRepsository = productImageRepsository;
        _productVariantRepsository = productVariantRepsository;
        _mapper = mapper;
    }
}