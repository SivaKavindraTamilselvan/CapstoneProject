using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<ResponseCartItemsDTO> DeleteCart(RequestDeleteCartItemsDTO requestDeleteCartItemsDTO)
    {
        var cartItems = await _cartValidation.ValidateCartItems(requestDeleteCartItemsDTO.CartItemsId);
        await _cartItemsRepsository.Delete(cartItems.CartItemsId);
        return _mapper.Map<ResponseCartItemsDTO>(cartItems);
    }
    public async Task<List<ResponseCartItemsDTO>> DeleteAllCart(int userId)
    {
        var cartItems = await _cartValidation.ValidateDeleteCartItemsByUserId(userId);
        return _mapper.Map<List<ResponseCartItemsDTO>>(cartItems);
    }
}