using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CartItemsRepsository : AbstractRepository<int, CartItems>, ICartItemsRepsository
{
    public CartItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<CartItems?> GetCartItemsByProductVariantAndCart(int productVariantId,int cartId)
    {
        var items = await _ecommerceContext.CartItems.FirstOrDefaultAsync(p=>p.ProductVariantId == productVariantId && p.CartId == cartId);
        return items;
    }
    public async Task<List<CartItems>> DeleteCartItemsByUserId(int userId)
    {
        var items = await _ecommerceContext.CartItems.Where(u => u.Cart != null && u.Cart.UserId == userId).ToListAsync();
        _ecommerceContext.CartItems.RemoveRange(items);
        await _ecommerceContext.SaveChangesAsync();
        return items;
    }

    public async Task<List<CartItems>> GetCartItemsByUserId(int userId)
    {
        var items = await _ecommerceContext.CartItems.Include(c => c.Cart).Include(p=>p.ProductVariant).ThenInclude(p=>p!.Product).Include(c => c.ProductVariant).ThenInclude(pv => pv!.Inventories).ThenInclude(i => i.Address).Where(u => u.Cart != null && u.Cart.UserId == userId).ToListAsync();
        return items;
    }

}