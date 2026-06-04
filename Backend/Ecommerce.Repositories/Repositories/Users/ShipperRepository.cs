using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ShipperRepsository : AbstractRepository<int, Shipper> ,IShipperRepsository
{
    public ShipperRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}