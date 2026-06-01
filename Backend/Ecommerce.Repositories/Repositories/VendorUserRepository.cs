using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class VendorUserRepsository : AbstractRepository<int, VendorUser> ,IVendorUserRepsository
{
    public VendorUserRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}