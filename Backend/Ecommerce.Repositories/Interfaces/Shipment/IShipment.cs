using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IShipmentRepsository : IRepository<int,Shipment>
{
            public Task<Shipment?> GetShipmentByOrderItemId(int orderitemid);

}