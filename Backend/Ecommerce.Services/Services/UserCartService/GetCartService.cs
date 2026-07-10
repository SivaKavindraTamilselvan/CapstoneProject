using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<List<ResponseGetCartDTO>> GetCartByUserId(int userId)
    {
        var cartItems = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        return _mapper.Map<List<ResponseGetCartDTO>>(cartItems);
    }
}