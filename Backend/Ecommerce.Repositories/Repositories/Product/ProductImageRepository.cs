using System.Collections.Frozen;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductImageRepsository : AbstractRepository<int, ProductImage>, IProductImageRepsository
{
    public ProductImageRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<ProductImage>> GetAllProductImageByProductId(int productId)
    {
        var product = await _ecommerceContext.ProductImage.Where(p => p.ProductId == productId).ToListAsync();
        return product;
    }
    public async Task<ProductImage?> GetProductImageByImageURL(string url)
    {
        return await _ecommerceContext.ProductImage.Where(p => p.ImageUrl == url).FirstOrDefaultAsync();
    }

    public async Task<ProductImage?> GetMainImageByProduct(int productId)
    {
        var product = await _ecommerceContext.ProductImage.Where(p => p.ProductId == productId && p.IsMainImage == true).FirstOrDefaultAsync();
        return product;
    }

}