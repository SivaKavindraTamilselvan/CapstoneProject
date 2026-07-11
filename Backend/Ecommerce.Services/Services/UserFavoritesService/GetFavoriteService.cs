using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<List<ResponseGetFavoriteDTO>> GetFavoriteByUserId(int userId)
    {
        var favoriteItems = await _favoriteValidation.ValidateGetFavoriteItemsByUserId(userId);
        var result = _mapper.Map<List<ResponseGetFavoriteDTO>>(favoriteItems);
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