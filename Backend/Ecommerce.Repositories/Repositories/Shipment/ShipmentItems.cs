using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentRepsository : AbstractRepository<int, Shipment>, IShipmentRepsository
{
    public ShipmentRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Shipment?> GetShipmentByOrderItemId(int orderitemid)
    {
        var shipment = await _ecommerceContext.Shipment.Include(s=>s.ShipmentItems).Include(s=>s!.ShipmentStatus).Where(s=>s.ShipmentItems.Any(si=>si.OrderItemsId == orderitemid)).FirstOrDefaultAsync();
        return shipment;
    }

}