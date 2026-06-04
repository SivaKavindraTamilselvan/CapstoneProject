using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorService : IVendorService
{
    private readonly IAuthentication _authentication;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IMapper _mapper;


    public VendorService(IMapper mapper,EcommerceContext ecommerceContext, IAuthentication authentication,IVendorUserRepsository vendorUserRepsository)
    {
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _vendorUserRepsository = vendorUserRepsository;
        _mapper = mapper;
    }
}