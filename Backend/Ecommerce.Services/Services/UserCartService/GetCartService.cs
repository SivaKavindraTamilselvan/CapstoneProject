using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    public async Task<List<ResponseGetCartDTO>> GetCartByUserId(int userId)
    {
        var cartItems = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        var result = _mapper.Map<List<ResponseGetCartDTO>>(cartItems);
        foreach (var cart in result)
        {
            var product = await _productVariantRepsository.GetProductByProductVariant(cart.ProductVariantId);
            if (product != null)
            {
                var image = await _productImageRepsository.GetMainImageByProduct(product.ProductId);
                if (image != null)
                {
                    cart.mainImageUrl = image.ImageUrl;
                }
            }
        }
        return result;
    }
}