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

}