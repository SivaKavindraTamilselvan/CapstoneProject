using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> UpdateCart(RequestUpdateCartItemsDTO requestUpdateCartItemsDTO, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            // validate the user if active and found
            await _userValidation.ValidateUser(userId);
            // validate the cart by user id
            await _cartValidation.ValidateCartByUserId(userId);

            _logger.LogInformation("Updating cart item. CartItemsId: {CartItemsId}, NewQuantity: {Quantity}", requestUpdateCartItemsDTO.CartItemsId, requestUpdateCartItemsDTO.Qunatity);

            var cartItems = (await _cartItemsRepsository.GetAll()).FirstOrDefault(c => c.CartItemsId == requestUpdateCartItemsDTO.CartItemsId);
            if (cartItems == null)
            {
                _logger.LogWarning("Cart item not found. CartItemsId: {CartItemsId}", requestUpdateCartItemsDTO.CartItemsId);
                throw new DataNotFoundException("Cart Items Is Not Found");
            }

            var cart = await _cartRepsository.Get(cartItems.CartId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found. CartId: {CartId}", cartItems.CartId);
                throw new DataNotFoundException("Cart Is Not Found");
            }

            var oldQuantity = cartItems.Qunatity;
            cartItems.Qunatity = requestUpdateCartItemsDTO.Qunatity;

            var updatedCartItems = await _cartItemsRepsository.Update(cartItems.CartItemsId, cartItems);
            if (updatedCartItems == null)
            {
                _logger.LogError("Failed to update CartItemsId {CartItemsId}", cartItems.CartItemsId);
                throw new DataRegistrationException("Updation of the cart item failed");
            }
            _logger.LogInformation("Updated CartItems table. CartItemsId: {CartItemsId}, Quantity: {OldQuantity} -> {NewQuantity}", updatedCartItems.CartItemsId, oldQuantity, updatedCartItems.Qunatity);

            var logChanges = new LogChanges
            {
                TableName = nameof(CartItems),
                RecordId = updatedCartItems.CartItemsId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"CartItemsId={cartItems.CartItemsId}, CartId={cartItems.CartId}, ProductVariantId={cartItems.ProductVariantId}, Quantity={oldQuantity}",
                NewValue = $"CartItemsId={updatedCartItems.CartItemsId}, CartId={updatedCartItems.CartId}, ProductVariantId={updatedCartItems.ProductVariantId}, Quantity={updatedCartItems.Qunatity}",
                UserId = cart.UserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);

            var oldUpdatedAt = cart.UpdatedAt;
            cart.UpdatedAt = DateTime.UtcNow;

            var updatedCart = await _cartRepsository.Update(cart.CartId, cart);
            if (updatedCart == null)
            {
                _logger.LogError("Failed to update CartId {CartId}", cart.CartId);
                throw new DataRegistrationException("Updation of the cart failed");
            }
            _logger.LogInformation("Updated Cart table. CartId: {CartId}, UpdatedAt: {UpdatedAt}", updatedCart.CartId, updatedCart.UpdatedAt);

            var cartLogChanges = new LogChanges
            {
                TableName = nameof(Cart),
                RecordId = updatedCart.CartId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"UpdatedAt={oldUpdatedAt:O}",
                NewValue = $"UpdatedAt={updatedCart.UpdatedAt:O}",
                UserId = updatedCart.UserId,
                ChangedAt = DateTime.Now
            };

            var createdCartLog = await _logChanges.Create(cartLogChanges);
            if (createdCartLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", cartLogChanges.TableName, cartLogChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", cartLogChanges.TableName, cartLogChanges.RecordId);

            await transaction.CommitAsync();
            _logger.LogInformation("Successfully updated cart item. CartItemsId: {CartItemsId}", updatedCartItems.CartItemsId);

            return _mapper.Map<ResponseCartItemsDTO>(updatedCartItems);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while updating CartItemsId {CartItemsId}", requestUpdateCartItemsDTO.CartItemsId);
            _logger.LogInformation("Transaction rolled back while updating CartItemsId {CartItemsId}", requestUpdateCartItemsDTO.CartItemsId);
            throw;
        }
    }
}