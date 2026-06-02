using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseAddCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO,int userId)
    {
        var cart = (await _cartRepsository.GetAll()).FirstOrDefault(f=>f.UserId == userId);
        var product = (await _cartItemsRepsository.GetAll()).FirstOrDefault(f=>f.ProductVariantId == requestAddCartItemsDTO.ProductVariantId && f.CartId == cart.CartId);
        if(product != null)
        {
            throw new Exception("Product Already Added To Favorites");
        }
        CartItems cartItems = new CartItems();
        cartItems.CartId = cart.CartId;
        cartItems.ProductVariantId = requestAddCartItemsDTO.ProductVariantId;
        await _cartItemsRepsository.Create(cartItems);
        return _mapper.Map<ResponseAddCartItemsDTO>(cartItems);

    }
}