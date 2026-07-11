using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductImageRepsository : IRepository<int,ProductImage>
{
    public Task<ProductImage?> GetMainImageByProduct(int productVariantId);
    public Task<ProductImage?> GetProductImageByImageURL(string url);
    public Task<List<ProductImage>> GetAllProductImageByProductId(int productId);
}