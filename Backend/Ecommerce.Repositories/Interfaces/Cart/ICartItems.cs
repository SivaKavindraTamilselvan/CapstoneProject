using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICartItemsRepsository : IRepository<int, CartItems>
{
    public  Task<Cart?> GetCartByCartItemId(int cartItemId);
    public Task<List<CartItems>> DeleteCartItemsByUserId(int userId);
    public Task<List<CartItems>> GetCartItemsByUserId(int userId);
    public Task<CartItems?> GetCartItemsByProductVariantAndCart(int productVariantId,int cartId);
}