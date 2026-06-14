using Ecommerce.DTOs;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IShipmentService
{
    public Task<ShipmentDetailResponseDto> GetShipmentDetailForAdmin(int shipmentId);
    public Task<List<ShipmentDetailResponseDto>> GetAllShipmentsForAdmin(RequestShipmentFilter filter);
    public Task<ShipmentItems> CreateShipmentItems(int shipmentId,int orderitemid);
    public Task<Shipment> CreateShipment(RequestAddShipmentDTO requestAddShipmentDTO);
    public Task<ResponseAddShipmentTrackingDTO> CreateShipmentTracking(RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO);
}