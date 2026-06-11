using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    public async Task<ResponseGetAddressDTO> DeleteUserAddress(int addressId, int userId)
    {
        _logger.LogInformation("User {UserId} requested deletion of AddressId {AddressId}", userId, addressId);
        await _userValidation.ValidateUser(userId);
        var selectedAddress = await _userValidation.ValidateAddress(addressId, userId);
        var orders = await _orderRepsository.GetPendingOrdersByAddress(addressId);
        if (orders.Count != 0)
        {
            _logger.LogWarning("AddressId {AddressId} cannot be deleted because pending orders exist", addressId);
            throw new DataApprovalStatusException("Address cannot be deleted as there is a current order right now");
        }
        if (selectedAddress.IsDefault)
        {
            _logger.LogWarning("Default AddressId {AddressId} cannot be deleted", addressId);
            throw new DataApprovalStatusException("Default address cannot be deleted");
        }
        selectedAddress.IsActive = false;
        selectedAddress.UpdatedAt = DateTime.Now;
        await _addressRepsository.Update(addressId, selectedAddress);
        _logger.LogInformation("AddressId {AddressId} successfully deactivated by UserId {UserId}", addressId, userId);
        await _notificationService.SendToUser(
            userId,
            "Address Deleted",
            $"Your address in {selectedAddress.City}, {selectedAddress.State} has been deleted.",
            notificationTypeId: 1,
            referenceType: "Address",
            referenceId: selectedAddress.AddressId);
        return _mapper.Map<ResponseGetAddressDTO>(selectedAddress);
    }
    public async Task<ResponseGetAddressDTO> DeleteInventoryAddress(int addressId, int userId)
    {
        _logger.LogInformation("Vendor UserId {UserId} requested deletion of Inventory AddressId {AddressId}", userId, addressId);
        await _userValidation.ValidateUser(userId);
        await _vendorUserValidation.ValidateVendorUserByUserId(userId);
        var selectedAddress = await _userValidation.ValidateAddress(addressId, userId);
        var orders = await _orderItemRepsository.GetPendingOrderByInventoryAddress(addressId);
        if (orders.Count != 0)
        {
            _logger.LogWarning("Inventory AddressId {AddressId} cannot be deleted because pending orders exist", addressId);
            throw new DataApprovalStatusException("Address cannot be deleted as there is a current order right now");
        }
        if (selectedAddress.IsDefault)
        {
            _logger.LogWarning("Default Inventory AddressId {AddressId} cannot be deleted", addressId);
            throw new DataApprovalStatusException("Default address cannot be deleted");
        }
        selectedAddress.IsActive = false;
        selectedAddress.UpdatedAt = DateTime.Now;
        await _addressRepsository.Update(addressId, selectedAddress);
        _logger.LogInformation("Inventory AddressId {AddressId} successfully deactivated by UserId {UserId}", addressId, userId);
        await _notificationService.SendToUser(
            userId,
            "Inventory Address Deleted",
            $"Your inventory address in {selectedAddress.City}, {selectedAddress.State} has been deleted.",
            notificationTypeId: 1,
            referenceType: "Address",
            referenceId: selectedAddress.AddressId);
        return _mapper.Map<ResponseGetAddressDTO>(selectedAddress);
    }

}