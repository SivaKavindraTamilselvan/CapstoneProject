using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} is registering a new vendor user with Email {Email} and VendorRoleId {VendorRoleId}", vendorUserId, requestRegisterVendorUserDTO.requestRegisterUserDTO.Email, requestRegisterVendorUserDTO.VendorRoleId);
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var user = await _authentication.RegisterUser(requestRegisterVendorUserDTO.requestRegisterUserDTO, (int)RoleEnum.Vendor);

            _logger.LogInformation("User created successfully. UserId {UserId}", user.UserId);
            var OwnerVendor = (await _vendorUserRepsository.GetAll()).FirstOrDefault(u => u.UserId == vendorUserId);
            if (OwnerVendor == null)
            {
                _logger.LogWarning("Vendor not found for UserId {VendorUserId}", vendorUserId);
                throw new DataNotFoundException("Vendor Not Found");
            }
            VendorUser vendorUser = new VendorUser();
            vendorUser.VendorId = OwnerVendor.VendorId;
            vendorUser.UserId = user.UserId;
            vendorUser.VendorRoleId = requestRegisterVendorUserDTO.VendorRoleId;
            vendorUser.AddedByVendorUserId = OwnerVendor.VendorUserId;
            await _vendorUserRepsository.Create(vendorUser);
            _logger.LogInformation("VendorUser created successfully. VendorUserId {VendorUserId}, VendorId {VendorId}, UserId {UserId}",
            vendorUser.VendorUserId,
            vendorUser.VendorId,
            vendorUser.UserId);
            await transaction.CommitAsync();
            _logger.LogInformation("Vendor user registration completed successfully by Vendor UserId {VendorUserId}", vendorUserId);
            return _mapper.Map<ResponseRegisterVendorUserDTO>(vendorUser); // authentication mapper
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering vendor user by Vendor UserId {VendorUserId}", vendorUserId);
            await transaction.RollbackAsync();
            throw;
        }
    }
}