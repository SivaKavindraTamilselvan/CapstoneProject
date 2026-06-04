using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentRepsository : AbstractRepository<int, Shipment>, IShipmentRepsository
{
    public ShipmentRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}