using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class AttributeRepsository : AbstractRepository<int, AttributeMaster>, IAttributeRepsository
{
    public AttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    // condition for validation before inserting
    public async Task<AttributeMaster?> GetTheAttributeByName(string attributeName)
    {
        return await _ecommerceContext.AttributeMaster.FirstOrDefaultAsync(a => a.AttributeName == attributeName);
    }

    // used for admin no needed for user or vendor as the attribute list is not needed for them
    public async Task<(List<AttributeMaster> items, int totalCount)> GetAllAttributeAdmin(RequestAttributeFilter request)
    {
        var query = _ecommerceContext.AttributeMaster.Include(a => a.AddedByAdminUser).ThenInclude(u => u!.User).AsQueryable();
        if (request.status.HasValue)
        {
            query = query.Where(a => a.IsActive == request.status);
        }
        if (request.AddedByAdminId.HasValue)
        {
            query = query.Where(a => a.AddedByAdminId == request.AddedByAdminId.Value);
        }
         if (!string.IsNullOrWhiteSpace(request.AttributeName))
        {
            query = query.Where(a => a.AttributeName.ToLower() == request.AttributeName.ToLower());
        }
        var totalCount = await query.CountAsync();
        var data = await query.OrderBy(a => a.AttributeName).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (data, totalCount);
    }
    public async Task<(List<AttributeMaster> items, int totalCount)> GetAllAttributeVendor()
    {
        var query = _ecommerceContext.AttributeMaster.Include(a => a.AddedByAdminUser).ThenInclude(u => u!.User).AsQueryable();
        query = query.Where(a => a.IsActive == true);
        var totalCount = await query.CountAsync();
        var data = await query.OrderBy(a => a.AttributeName).ToListAsync();
        return (data, totalCount);
    }
}