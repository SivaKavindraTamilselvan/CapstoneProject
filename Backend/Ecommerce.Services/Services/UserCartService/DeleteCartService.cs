using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> DeleteCart(RequestDeleteCartItemsDTO requestDeleteCartItemsDTO,int userId)
    {
        _logger.LogInformation("Deleting CartItemId {CartItemId}.", requestDeleteCartItemsDTO.CartItemsId);

        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            // validate the user if active and found
            await _userValidation.ValidateUser(userId);
            // validate the cart by user id
            var cart = await _cartValidation.ValidateCartByUserId(userId);
            _logger.LogInformation("Cart validation successful. CartId {CartId} found for UserId {UserId}.", cart.CartId, userId);

            var cartItems = await _cartValidation.ValidateCartItems(requestDeleteCartItemsDTO.CartItemsId);
            await _cartItemsRepsository.Delete(cartItems.CartItemsId);
            _logger.LogInformation("CartItemId {CartItemId} deleted successfully.",cartItems.CartItemsId);

            var logChanges = new LogChanges
            {
                TableName = nameof(CartItems),
                RecordId = cartItems.CartItemsId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"CartItemsId={cartItems.CartItemsId}, CartId={cartItems.CartId}, ProductVariantId={cartItems.ProductVariantId}",
                NewValue = string.Empty,
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);

            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for CartItemId {CartItemId}.",cartItems.CartItemsId);
                throw new DataRegistrationException("Failed to create audit log.");
            }

            _logger.LogInformation("Audit log created successfully for CartItemId {CartItemId}.", cartItems.CartItemsId);
            await transaction.CommitAsync();

            _logger.LogInformation("Delete cart operation completed successfully for CartItemId {CartItemId}.",cartItems.CartItemsId);
            return _mapper.Map<ResponseCartItemsDTO>(cartItems);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex,"Error occurred while deleting CartItemId {CartItemId}.",requestDeleteCartItemsDTO.CartItemsId);
            throw;
        }
    }

    public async Task<List<ResponseCartItemsDTO>> DeleteAllCart(int userId)
    {
        _logger.LogInformation("Deleting all cart items for UserId {UserId}.",userId);
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
             // validate the user if active and found
            await _userValidation.ValidateUser(userId);
            var cartItems = await _cartValidation.ValidateDeleteCartItemsByUserId(userId);

            foreach (var item in cartItems)
            {
                await _cartItemsRepsository.Delete(item.CartItemsId);

                _logger.LogInformation("Deleted CartItemId {CartItemId}.",item.CartItemsId);

                var logChanges = new LogChanges
                {
                    TableName = nameof(CartItems),
                    RecordId = item.CartItemsId,
                    Actions = (int)AuditAction.Deleted,
                    OldValue =$"CartItemsId={item.CartItemsId}, CartId={item.CartId}, ProductVariantId={item.ProductVariantId}",
                    NewValue = string.Empty,
                    UserId = userId,
                    ChangedAt = DateTime.Now
                };

                var createdLog = await _logChanges.Create(logChanges);

                if (createdLog == null)
                {
                    _logger.LogError("Failed to create audit log for CartItemId {CartItemId}.", item.CartItemsId);
                    throw new DataRegistrationException("Failed to create audit log.");
                }
            }

            await transaction.CommitAsync();

            _logger.LogInformation("Successfully deleted {Count} cart item(s) for UserId {UserId}.",cartItems.Count,userId);

            return _mapper.Map<List<ResponseCartItemsDTO>>(cartItems);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(  ex,  "Error occurred while deleting all cart items for UserId {UserId}.",  userId);
            throw;
        }
    }
}