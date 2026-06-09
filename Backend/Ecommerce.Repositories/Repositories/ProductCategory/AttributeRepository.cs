using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AttributeRepsository : AbstractRepository<int, AttributeMaster>, IAttributeRepsository
{
    public AttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<AttributeMaster?> GetTheAttributeByName(string attributeName)
    {
        return await _ecommerceContext.AttributeMaster.FirstOrDefaultAsync(a => a.AttributeName == attributeName);
    }
    public async Task<(List<AttributeMaster> ,int totalCount)> GetAllAttributeAdmin(bool? status,int pageNumber,int pageSize)
    {
        var query = _ecommerceContext.AttributeMaster.Include(a=>a.AddedByAdminUser).ThenInclude(u=>u!.User).AsQueryable();
        if(status.HasValue)
        {
            query = query.Where(a=>a.IsActive == status);
        }
        var totalCount = await query.CountAsync();
        var data =  await query.OrderBy(a=>a.AttributeName).Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        return (data,totalCount);
    }
}