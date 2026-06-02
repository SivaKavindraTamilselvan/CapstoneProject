using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CartItemsRepsository : AbstractRepository<int, CartItems>, ICartItemsRepsository
{
    public CartItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<CartItems>> DeleteCartItemsByUserId(int userId)
    {
        var items = await _ecommerceContext.CartItems.Include(c => c.Cart).Where(u => u.Cart != null && u.Cart.UserId == userId).ToListAsync();
        if (items.Count == 0)
        {
            return new List<CartItems>();
        }
        _ecommerceContext.RemoveRange(items);
        await _ecommerceContext.SaveChangesAsync();
        return items;
    }

}