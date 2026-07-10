using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class CartValidation : ICartValidation
{
    private readonly ICartRepsository _cartRepsository;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    public CartValidation(ICartRepsository cartRepsository, ICartItemsRepsository cartItemsRepsository)
    {
        _cartRepsository = cartRepsository;
        _cartItemsRepsository = cartItemsRepsository;
    }
    public async Task<Cart> ValidateCartByUserId(int userId)
    {
        var cart = await _cartRepsository.GetCartByUserId(userId);
        if (cart == null)
        {
            throw new DataNotFoundException("Cart Not Found");
        }
        return cart;
    }
    public async Task<CartItems> ValidateCartItems(int cartItemId)
    {
        var cartItems = await _cartItemsRepsository.Get(cartItemId);
        if(cartItems ==null)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        return cartItems;
    }
    public async Task ValidateCartItemsByProductAndUser(int productVariantId, int cartId)
    {
        var cartItem = await _cartItemsRepsository.GetCartItemsByProductVariantAndCart(productVariantId, cartId);
        if (cartItem != null)
        {
            throw new DataNotFoundException("Product Already Added To Cart");
        }
    }
    public async Task<List<CartItems>> ValidateDeleteCartItemsByUserId(int userId)
    {
        var cartItems = await _cartItemsRepsository.DeleteCartItemsByUserId(userId);
        if(cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Is Empty");
        }
        return cartItems;
    }
    public async Task<List<CartItems>> ValidateGetCartItemsByUserId(int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if(cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        return cartItems;
    }
}
