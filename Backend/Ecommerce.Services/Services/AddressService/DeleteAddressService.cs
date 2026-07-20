using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    public async Task<ResponseGetAddressDTO> DeleteUserAddress(int addressId, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("User {UserId} requested deletion of AddressId {AddressId}", userId, addressId);
            // validate the user is active and found and address belongs to that user
            var selectedAddress = await _userValidation.ValidateAddress(addressId, userId);

            // prevent user from deleting the address if there is a current order placed in that address
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

            var logChanges = new LogChanges
            {
                TableName = nameof(Address),
                RecordId = selectedAddress.AddressId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=True",
                NewValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=False",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            await _logChanges.Create(logChanges);
            _logger.LogInformation("Audit log created for AddressId {AddressId}", selectedAddress.AddressId);

            await transaction.CommitAsync();

            return _mapper.Map<ResponseGetAddressDTO>(selectedAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deleting AddressId {AddressId}", addressId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for AddressId {AddressId}", addressId);
            throw;
        }
    }
    public async Task<ResponseGetAddressDTO> DeleteInventoryAddress(int addressId, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {UserId} requested deletion of Inventory AddressId {AddressId}", userId, addressId);
            await _vendorUserValidation.ValidateOwnerVendorUserByUserId(userId);
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

            var logChanges = new LogChanges
            {
                TableName = nameof(Address),
                RecordId = selectedAddress.AddressId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=True",
                NewValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=False",
                UserId = userId,
                ChangedAt = DateTime.UtcNow
            };

            await _logChanges.Create(logChanges);

            _logger.LogInformation("Audit log created for Inventory AddressId {AddressId}", selectedAddress.AddressId);

            await _notificationService.SendToUser(
                userId,
                "Inventory Address Deleted",
                $"Your inventory address in {selectedAddress.City}, {selectedAddress.State} has been deleted.",
                notificationTypeId: (int)NotificationTypeEnum.WarehouseDeleted,
                referenceType: "Address",
                referenceId: selectedAddress.AddressId);
                
            await transaction.CommitAsync();
            
            return _mapper.Map<ResponseGetAddressDTO>(selectedAddress);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deleting AddressId {AddressId}", addressId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for AddressId {AddressId}", addressId);
            throw;
        }
    }
}