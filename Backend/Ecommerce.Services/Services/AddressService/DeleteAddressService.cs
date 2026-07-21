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

            var updatedAddress = await _addressRepsository.Update(addressId, selectedAddress);
            if (updatedAddress == null)
            {
                _logger.LogError("Failed to update AddressId {AddressId}", addressId);
                throw new DataRegistrationException("Failed to update address");
            }
            _logger.LogInformation("AddressId {AddressId} successfully deactivated by UserId {UserId}", addressId, userId);

            var logChanges = new LogChanges
            {
                TableName = nameof(Address),
                RecordId = updatedAddress.AddressId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=True",
                NewValue = $"AddressId={updatedAddress.AddressId}, UserId={updatedAddress.UserId}, AddressLine={updatedAddress.AddressLine}, City={updatedAddress.City}, State={updatedAddress.State}, PinCode={updatedAddress.PinCode}, IsActive=False",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for AddressId {AddressId}", updatedAddress.AddressId);

            await transaction.CommitAsync();

            return _mapper.Map<ResponseGetAddressDTO>(updatedAddress);
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

            var updatedAddress = await _addressRepsository.Update(addressId, selectedAddress);
            if (updatedAddress == null)
            {
                _logger.LogError("Failed to update Inventory AddressId {AddressId}", addressId);
                throw new DataRegistrationException("Failed to update address");
            }
            _logger.LogInformation("Inventory AddressId {AddressId} successfully deactivated by UserId {UserId}", addressId, userId);

            var logChanges = new LogChanges
            {
                TableName = nameof(Address),
                RecordId = updatedAddress.AddressId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"AddressId={selectedAddress.AddressId}, UserId={selectedAddress.UserId}, AddressLine={selectedAddress.AddressLine}, City={selectedAddress.City}, State={selectedAddress.State}, PinCode={selectedAddress.PinCode}, IsActive=True",
                NewValue = $"AddressId={updatedAddress.AddressId}, UserId={updatedAddress.UserId}, AddressLine={updatedAddress.AddressLine}, City={updatedAddress.City}, State={updatedAddress.State}, PinCode={updatedAddress.PinCode}, IsActive=False",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for Warehouse AddressId {AddressId}", updatedAddress.AddressId);

            await _notificationService.SendToUser(
                userId,
                "Warehouse Address Deleted",
                $"Your inventory address in {updatedAddress.City}, {updatedAddress.State} has been deleted.",
                notificationTypeId: (int)NotificationTypeEnum.WarehouseDeleted,
                referenceType: "Address",
                referenceId: updatedAddress.AddressId);

            await transaction.CommitAsync();

            return _mapper.Map<ResponseGetAddressDTO>(updatedAddress);
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