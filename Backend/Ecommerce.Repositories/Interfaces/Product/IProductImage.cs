using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IProductImageRepsository : IRepository<int,ProductImage>
{
    public Task<List<ProductImage>> GetAllProductImageByProductId(int productId);
}