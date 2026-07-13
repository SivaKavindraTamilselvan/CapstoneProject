using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ReviewRepsository : AbstractRepository<int, Reviews> ,IReviewRepsository
{
    public ReviewRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<List<Reviews>> GetByProductId(int productId)
    {
        return await _ecommerceContext.Reviews
            .Include(r => r.Star)
            .Include(r => r.ReviewDescription)
            .Include(r => r.OrderItems)
                .ThenInclude(oi => oi!.Order) // adjust based on your actual nav chain to User
            .Where(r => r.OrderItems != null && r.OrderItems.ProductVariant.Product.ProductId == productId) // adjust FK name if different
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

}