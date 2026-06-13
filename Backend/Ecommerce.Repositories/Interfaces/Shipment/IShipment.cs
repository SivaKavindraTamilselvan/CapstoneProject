using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IShipmentRepsository : IRepository<int, Shipment>
{
    public Task<(List<Shipment> Items, int TotalCount)> GetAllShipmentsForAdmin(RequestShipmentFilter filter);
    public Task<Shipment?> GetShipmentDetailForAdmin(int shipmentId);

}