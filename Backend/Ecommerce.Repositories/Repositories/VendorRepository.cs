using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class VendorRepsository : AbstractRepository<int, Vendor> ,IVendorRepsository
{
    public VendorRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}