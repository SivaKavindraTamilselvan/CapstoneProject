using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorService : IVendorService
{
    public async Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO,int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var user = await _authentication.RegisterUser(requestRegisterVendorUserDTO.requestRegisterUserDTO, (int)RoleEnum.Vendor);

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
            return _mapper.Map<ResponseRegisterVendorUserDTO>(vendorUser); // authentication mapper
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}