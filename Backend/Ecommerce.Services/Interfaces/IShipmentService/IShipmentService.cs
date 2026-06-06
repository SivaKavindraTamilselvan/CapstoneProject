using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IShipmentService
{
    public Task<ShipmentItems> CreateShipmentItems(int shipmentId,int orderitemid);
    public Task<ResponseAddShipmentDTO> CreateShipment(RequestAddShipmentDTO requestAddShipmentDTO);
    public Task<ResponseAddShipmentTrackingDTO> CreateShipmentTracking(RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO);
}