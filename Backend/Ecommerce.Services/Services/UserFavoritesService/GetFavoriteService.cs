using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<List<ResponseGetFavoriteDTO>> GetFavoriteByUserId(int userId)
    {
        _logger.LogInformation("Getting favorite items for UserId: {UserId}", userId);

        var favoriteItems = await _favoriteValidation.ValidateGetFavoriteItemsByUserId(userId);

        _logger.LogInformation("Retrieved {FavoriteItemCount} favorite items for UserId: {UserId}", favoriteItems.Count, userId);

        var result = _mapper.Map<List<ResponseGetFavoriteDTO>>(favoriteItems);

        foreach (var favorite in result)
        {
            _logger.LogInformation("Loading product details for ProductVariantId: {ProductVariantId}", favorite.ProductVariantId);

            var product = await _productVariantRepsository.GetProductByProductVariant(favorite.ProductVariantId);

            if (product != null)
            {
                var image = await _productImageRepsository.GetMainImageByProduct(product.ProductId);

                if (image != null)
                {
                    favorite.mainImageUrl = image.ImageUrl;
                }
            }
        }

        _logger.LogInformation("Successfully returned favorite items for UserId: {UserId}", userId);

        return result;
    }
}