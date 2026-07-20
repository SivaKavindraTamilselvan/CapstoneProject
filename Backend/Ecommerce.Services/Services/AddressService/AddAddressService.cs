using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    public async Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO, int UserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Adding address for UserId {UserId}", UserId);
            //checked if the user is active and found
            var user = await _userValidation.ValidateUser(UserId);

            // if vendor then only vendor owner can add waehouse
            if (user.RoleId == (int)RoleEnum.Vendor)
            {
                var vendorUser = await _vendorUserRepsository.CheckUserIsVendorOwner(user.UserId);
                if (vendorUser == null)
                {
                    throw new UnauthorizationException("Only Vendor Owner can add address");
                }
            }
            var address = _mapper.Map<Address>(requestAddAddressDTO);
            address.UserId = UserId;
            var createdAddress = await _addressRepsository.Create(address);
            if (createdAddress == null)
            {
                _logger.LogError("Address creation failed for UserId {UserId}. City: {City}, Pincode: {Pincode}", UserId, address.City, address.PinCode);
                throw new DataRegistrationException("Failed to create address");
            }

            if (createdAddress.IsDefault)
            {
                _logger.LogInformation("Setting AddressId {AddressId} as default for UserId {UserId}", createdAddress.AddressId, UserId);
                await MakeAddressDefault(createdAddress.AddressId, UserId);
            }
            _logger.LogInformation("Address created successfully with AddressId {AddressId} for UserId {UserId}", createdAddress.AddressId, UserId);

            var logChanges = new LogChanges
            {
                TableName = nameof(Address),
                RecordId = createdAddress.AddressId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"AddressId={createdAddress.AddressId}, UserId={createdAddress.UserId}, AddressLine={createdAddress.AddressLine}, City={createdAddress.City}, State={createdAddress.State}, PinCode={createdAddress.PinCode}, IsDefault={createdAddress.IsDefault}",
                UserId = UserId,
                ChangedAt = DateTime.UtcNow
            };

            await _logChanges.Create(logChanges);

            _logger.LogInformation("Audit log created for AddressId {AddressId}", createdAddress.AddressId);

            // 

            if (user.RoleId == (int)RoleEnum.Vendor)
            {
                await _notificationService.SendToUser(
                    UserId,
                    "Warehouse Address Added",
                    $"Your inventory address in {createdAddress.City}, {createdAddress.State} has been created.",
                    notificationTypeId: (int)NotificationTypeEnum.WarehouseAdded,
                    referenceType: "Address",
                    referenceId: createdAddress.AddressId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddAddressDTO>(createdAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while adding address for UserId: {UserId}", UserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for UserId: {UserId}", UserId);
            throw;
        }
    }
    public async Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(int addressId, int userId)
    {
        _logger.LogInformation("Making AddressId {AddressId} default", addressId);
        // checked if that address belongs to that user
        // checked if user is valid beacause make default api can be called even after creation of address
        var selectedAddress = await _userValidation.ValidateAddress(addressId, userId);
        var userAddress = await _addressRepsository.GetAllAddressByUserId(selectedAddress.UserId);
        foreach (var address in userAddress)
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.Now;
            var updatedAddress = await _addressRepsository.Update(address.AddressId, address);
            if (updatedAddress == null)
            {
                _logger.LogError("Failed to update AddressId {AddressId}", address.AddressId);
                throw new DataRegistrationException("Failed to update address");
            }
        }
        selectedAddress.IsDefault = true;
        selectedAddress.UpdatedAt = DateTime.Now;
        await _addressRepsository.Update(selectedAddress.AddressId, selectedAddress);
        _logger.LogInformation("AddressId {AddressId} set as default successfully", selectedAddress.AddressId);
        return _mapper.Map<ResponseMakeDefaultAddressDTO>(selectedAddress);
    }
}