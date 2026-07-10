using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface ICartValidation
{
    public Task<Cart> ValidateCartByUserId(int userId);
    public Task ValidateCartItemsByProductAndUser(int productVariantId, int cartId);
    public Task<CartItems> ValidateCartItems(int cartItemId);
    public Task<List<CartItems>> ValidateDeleteCartItemsByUserId(int userId);
    public Task<List<CartItems>> ValidateGetCartItemsByUserId(int userId);
}