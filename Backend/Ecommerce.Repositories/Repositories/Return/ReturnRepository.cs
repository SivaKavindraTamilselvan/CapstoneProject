using System.Security.Cryptography.X509Certificates;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ReturnRepsository : AbstractRepository<int, Return> ,IReturnRepsository
{
    public ReturnRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Return?> GetTheReturnInventoryByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r=>r.OrderItems).ThenInclude(o=>o!.Inventory).ThenInclude(i=>i!.Address).Where(r=>r.ReturnId == returnId).FirstOrDefaultAsync();
    }
    public async Task<Return?> GetTheReturnUserByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r=>r.OrderItems).ThenInclude(o=>o!.Order).ThenInclude(o=>o!.Users).ThenInclude(a=>a!.Addresses).Where(r=>r.ReturnId == returnId).FirstOrDefaultAsync();
    }
    public async Task<Return?> GetTheReturnProductByReturnId(int returnId)
    {
        return await _ecommerceContext.Return.Include(r=>r.OrderItems).ThenInclude(o=>o!.ProductVariant).ThenInclude(p=>p!.Product).Where(r=>r.ReturnId == returnId).FirstOrDefaultAsync();
    }
}