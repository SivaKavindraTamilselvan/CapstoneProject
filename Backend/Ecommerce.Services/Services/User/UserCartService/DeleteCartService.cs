using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> DeleteCart(RequestDeleteCartItemsDTO requestDeleteCartItemsDTO)
    {
        var cartItems = (await _cartItemsRepsository.GetAll()).FirstOrDefault(c=>c.CartItemsId == requestDeleteCartItemsDTO.CartItemsId);
        if(cartItems ==null)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        await _cartItemsRepsository.Delete(cartItems.CartItemsId);
        return _mapper.Map<ResponseCartItemsDTO>(cartItems);
    }
    public async Task<List<ResponseCartItemsDTO>> DeleteAllCart()
    {
        var cartItems = await _cartItemsRepsository.GetAll();
        await _cartItemsRepsository.DeleteAll();
        return _mapper.Map<List<ResponseCartItemsDTO>>(cartItems);
    }
}