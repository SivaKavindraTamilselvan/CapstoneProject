using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> UpdateCart(RequestUpdateCartItemsDTO requestUpdateCartItemsDTO)
    {
        var cartItems = (await _cartItemsRepsository.GetAll()).FirstOrDefault(c=>c.CartItemsId == requestUpdateCartItemsDTO.CartItemsId);
        if(cartItems ==null)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        cartItems.Qunatity = requestUpdateCartItemsDTO.Qunatity;
        await _cartItemsRepsository.Update(cartItems.CartItemsId,cartItems);
        return _mapper.Map<ResponseCartItemsDTO>(cartItems);

    }
}