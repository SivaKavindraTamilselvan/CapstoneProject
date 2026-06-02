using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorService : IVendorService
{
    private readonly IAuthentication _authentication;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IMapper _mapper;


    public VendorService(IMapper mapper,EcommerceContext ecommerceContext, IAuthentication authentication,IVendorRepsository vendorRepsository,IVendorUserRepsository vendorUserRepsository,IProductRepsository productRepsository)
    {
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _vendorRepsository = vendorRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _productRepsository = productRepsository;
        _mapper = mapper;
    }
    public async Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO,int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var user = await _authentication.RegisterUser(requestRegisterVendorUserDTO.requestRegisterUserDTO, 2);

            var OwnerVendor = (await _vendorUserRepsository.GetAll()).FirstOrDefault(u=>u.UserId == vendorUserId);
            if(OwnerVendor == null)
            {
                throw new DataNotFoundException("Vendor Not Found");
            }
            VendorUser vendorUser = new VendorUser();
            vendorUser.VendorId = OwnerVendor.VendorId;
            vendorUser.UserId = user.UserId;
            vendorUser.VendorRoleId = requestRegisterVendorUserDTO.VendorRoleId;
            vendorUser.AddedByVendorUserId = OwnerVendor.VendorUserId;
            await _vendorUserRepsository.Create(vendorUser);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseRegisterVendorUserDTO>(vendorUser);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}