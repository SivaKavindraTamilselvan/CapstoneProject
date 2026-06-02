using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;

public class UserCartService : IUserCartService
{
    private readonly ICartRepsository _cartRepsository;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IMapper _mapper;

    public UserCartService(ICartItemsRepsository cartItemsRepsository,ICartRepsository cartRepsository,IMapper mapper)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _cartRepsository = cartRepsository;
        _mapper = mapper;
    }
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