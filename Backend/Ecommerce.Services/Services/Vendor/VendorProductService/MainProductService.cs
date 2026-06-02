using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IMapper _mapper;


    public VendorProductService(IMapper mapper,IVendorUserRepsository vendorUserRepsository,IVendorRepsository vendorRepsository,IProductRepsository productRepsository,IProductVariantRepsository productVariantRepsository,IProductImageRepsository productImageRepsository)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _vendorRepsository = vendorRepsository;
        _productRepsository = productRepsository;
        _productVariantRepsository = productVariantRepsository;
        _productImageRepsository = productImageRepsository;
        _mapper = mapper;
    }
}