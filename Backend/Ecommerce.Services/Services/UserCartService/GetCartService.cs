using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCartService : IUserCartService
{
    public async Task<List<ResponseGetCartDTO>> GetCartByUserId(int userId)
    {
        _logger.LogInformation("Getting cart items for UserId: {UserId}", userId);
        var cartItems = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        _logger.LogInformation("Retrieved {CartItemCount} cart items for UserId: {UserId}", cartItems.Count, userId);
        var result = _mapper.Map<List<ResponseGetCartDTO>>(cartItems);
        foreach (var cart in result)
        {
            _logger.LogInformation("Loading product details for ProductVariantId: {ProductVariantId}", cart.ProductVariantId);
            var product = await _productVariantRepsository.GetProductByProductVariant(cart.ProductVariantId);
            if (product != null)
            {
                _logger.LogInformation("Found ProductId: {ProductId} for ProductVariantId: {ProductVariantId}", product.ProductId, cart.ProductVariantId);
                var image = await _productImageRepsository.GetMainImageByProduct(product.ProductId);
                if (image != null)
                {
                    cart.mainImageUrl = image.ImageUrl;
                    _logger.LogInformation("Assigned main image for ProductId: {ProductId}", product.ProductId);
                }
                else
                {
                    _logger.LogWarning("Main image not found for ProductId: {ProductId}", product.ProductId);
                }
            }
            else
            {
                _logger.LogWarning("Product not found for ProductVariantId: {ProductVariantId}", cart.ProductVariantId);
            }
        }

        _logger.LogInformation("Successfully returned cart items for UserId: {UserId}", userId);
        return result;
    }
}