using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class AttributeRepsository : AbstractRepository<int, AttributeMaster> ,IAttributeRepsository
{
    public AttributeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}