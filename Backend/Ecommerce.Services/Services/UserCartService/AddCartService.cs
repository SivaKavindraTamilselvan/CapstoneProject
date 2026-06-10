using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO,int userId)
    {
        var cart = await _cartValidation.ValidateCartByUserId(userId);
        await _productValidation.ValidateProductVariantIfApproved(requestAddCartItemsDTO.ProductVariantId);
        await _cartValidation.ValidateCartItemsByProductAndUser(requestAddCartItemsDTO.ProductVariantId,cart.CartId);
        CartItems cartItems = new CartItems();
        cartItems.CartId = cart.CartId;
        cartItems.ProductVariantId = requestAddCartItemsDTO.ProductVariantId;
        await _cartItemsRepsository.Create(cartItems);
        return _mapper.Map<ResponseCartItemsDTO>(cartItems);
    }
}