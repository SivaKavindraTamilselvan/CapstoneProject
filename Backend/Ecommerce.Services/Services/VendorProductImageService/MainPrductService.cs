using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorProductImageService : IVendorProductImageService
{
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;


    public VendorProductImageService(IVendorUserValidation vendorUserValidation,IMapper mapper,IProductImageRepsository productImageRepsository,IProductValidation productValidation)
    {
        _vendorUserValidation = vendorUserValidation;
        _productImageRepsository = productImageRepsository;     
        _productValidation = productValidation;
        _mapper = mapper;
    }
}