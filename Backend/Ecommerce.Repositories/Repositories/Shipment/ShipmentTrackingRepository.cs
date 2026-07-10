using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentTrackingRepsository : AbstractRepository<int, ShipmentTracking>, IShipmentTrackingRepsository
{
    public ShipmentTrackingRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}