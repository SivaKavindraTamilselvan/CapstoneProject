using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO, int userId)
    {
        _logger.LogInformation("UserId {UserId} is adding ProductVariantId {ProductVariantId} to cart.", userId, requestAddCartItemsDTO.ProductVariantId);
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            // validate the user if active and found
            await _userValidation.ValidateUser(userId);
            // validate the cart by user id
            var cart = await _cartValidation.ValidateCartByUserId(userId);
            _logger.LogInformation("Cart validation successful. CartId {CartId} found for UserId {UserId}.", cart.CartId, userId);

            await _productValidation.ValidateProductVariantIfApproved(requestAddCartItemsDTO.ProductVariantId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} validated successfully.", requestAddCartItemsDTO.ProductVariantId);
            
            await _cartValidation.ValidateCartItemsByProductAndUser(requestAddCartItemsDTO.ProductVariantId, cart.CartId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} is not already present in CartId {CartId}.", requestAddCartItemsDTO.ProductVariantId, cart.CartId);
            CartItems cartItems = new CartItems
            {
                CartId = cart.CartId,
                ProductVariantId = requestAddCartItemsDTO.ProductVariantId
            };
            cart.UpdatedAt = DateTime.UtcNow;
            var updatedCart = await _cartRepsository.Update(cart.CartId, cart);
            if (updatedCart == null)
            {
                _logger.LogError("Failed to update CartId {CartId}.", cart.CartId);
                throw new DataRegistrationException("Failed to update cart.");
            }
            _logger.LogInformation("CartId {CartId} updated successfully.", cart.CartId);
            var createdCartItem = await _cartItemsRepsository.Create(cartItems);
            if (createdCartItem == null)
            {
                _logger.LogError("Failed to create cart item for CartId {CartId}, ProductVariantId {ProductVariantId}.", cart.CartId, requestAddCartItemsDTO.ProductVariantId);
                throw new DataRegistrationException("Failed to add item to cart.");
            }
            _logger.LogInformation("CartItem created successfully. CartItemId {CartItemId}.", createdCartItem.CartItemsId);
            var logChanges = new LogChanges
            {
                TableName = nameof(CartItems),
                RecordId = createdCartItem.CartItemsId,
                Actions = (int)AuditAction.Created,
                OldValue = "",
                NewValue = $"CartItemsId={createdCartItem.CartItemsId}, CartId={createdCartItem.CartId}, ProductVariantId={createdCartItem.ProductVariantId}",
                UserId = userId,
                ChangedAt = DateTime.Now
            };
            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for CartItemId {CartItemId}.", createdCartItem.CartItemsId);
                throw new DataRegistrationException("Failed to create audit log.");
            }
            _logger.LogInformation("Audit log created successfully for Table {TableName}, RecordId {RecordId}.", logChanges.TableName, logChanges.RecordId);
            await transaction.CommitAsync();
            _logger.LogInformation("UserId {UserId} successfully added ProductVariantId {ProductVariantId} to CartId {CartId}.", userId, requestAddCartItemsDTO.ProductVariantId, cart.CartId);
            return _mapper.Map<ResponseCartItemsDTO>(createdCartItem);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error occurred while UserId {UserId} was adding ProductVariantId {ProductVariantId} to cart.", userId, requestAddCartItemsDTO.ProductVariantId);
            throw;
        }
    }
}