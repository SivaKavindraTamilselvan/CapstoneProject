using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ShipmentService :IShipmentService
{
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IShipmentTrackingRepsository _shipmentTrackingRepsository;
    private readonly IMapper _mapper;
    public ShipmentService(IShipmentTrackingRepsository shipmentTrackingRepsository,IMapper mapper,IShipmentItemsRepsository shipmentItemsRepsository,IShipmentRepsository shipmentRepsository)
    {
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _shipmentRepsository = shipmentRepsository;
        _shipmentTrackingRepsository = shipmentTrackingRepsository;
        _mapper = mapper;
    }
    public async Task<Shipment> CreateShipment(RequestAddShipmentDTO requestAddShipmentDTO)
    {
        var shipment = _mapper.Map<Shipment>(requestAddShipmentDTO);
        var createdShipment = await _shipmentRepsository.Create(shipment);
        if (createdShipment == null)
        {
            throw new DataNotFoundException("Shipment not created");
        }
        return createdShipment;
    }
    public async Task<ResponseAddShipmentTrackingDTO> CreateShipmentTracking(RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO)
    {
        var shipmentTracking = _mapper.Map<ShipmentTracking>(requestAddShipmentTrackingDTO);
        await _shipmentTrackingRepsository.Create(shipmentTracking);
        return _mapper.Map<ResponseAddShipmentTrackingDTO>(shipmentTracking);
    }
    public async Task<ShipmentItems> CreateShipmentItems(int shipmentId,int orderitemid)
    {
        ShipmentItems shipmentItems = new ShipmentItems();
        shipmentItems.OrderItemsId = orderitemid;
        shipmentItems.ShipmentId = shipmentId;
        await _shipmentItemsRepsository.Create(shipmentItems);
        return shipmentItems;
    }
}