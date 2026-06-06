using System.Security.Cryptography.X509Certificates;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CartRepsository : AbstractRepository<int, Cart>, ICartRepsository
{
    public CartRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Cart?> GetCartByUserId(int userId)
    {
        var cart = await _ecommerceContext.Cart.FirstOrDefaultAsync(c=>c.UserId == userId);
        return cart;
    }

}