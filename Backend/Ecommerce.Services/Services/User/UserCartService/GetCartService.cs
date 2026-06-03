using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> GetCartByUserId(int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if(cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        return _mapper.Map<ResponseCartItemsDTO>(cartItems);
    }
}