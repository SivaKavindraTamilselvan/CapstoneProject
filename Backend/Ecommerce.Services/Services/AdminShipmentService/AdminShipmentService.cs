using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminShipmentService : IAdminShipmentService
{
    private readonly IShipmentService _shipmentService;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IMapper _mapper;
    public AdminShipmentService(IShipmentService shipmentService, IShipmentRepsository shipmentRepsository, IMapper mapper, IShipmentItemsRepsository shipmentItemsRepsository, IOrderItemRepsository orderItemRepsository, IOrderRepsository orderRepsository)
    {
        _shipmentService = shipmentService;
        _shipmentRepsository = shipmentRepsository;
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _orderItemRepsository = orderItemRepsository;
        _orderRepsository = orderRepsository;
        _mapper = mapper;
    }
    public async Task<ShipmentStatusResponseDTO> UpdateShimentStatus(ShipmentStatusRequestDTO shipmentStatusRequestDTO)
    {
        var shipment = await _shipmentRepsository.Get(shipmentStatusRequestDTO.ShipmentId);
        if (shipment == null)
        {
            throw new DataNotFoundException("Shipment Not Found");
        }
        shipment.ShipmentStatusId = shipmentStatusRequestDTO.ShipmentStatusId;
        if (shipment.ShipmentStatusId == 5)
        {
            shipment.ShippedDate = DateTime.Now;
            await UpdateOrderItemStatus(shipment.ShipmentId, 3);
        }
        if (shipment.ShipmentStatusId == 8)
        {
            shipment.DeliveryDate = DateTime.Now;
            await UpdateOrderItemStatus(shipment.ShipmentId, 4);

        }
        await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        RequestAddShipmentTrackingDTO requestAddShipmentTrackingDTO = new RequestAddShipmentTrackingDTO();
        requestAddShipmentTrackingDTO.ShipmentId = shipment.ShipmentId;
        requestAddShipmentTrackingDTO.Location = shipmentStatusRequestDTO.Location;
        requestAddShipmentTrackingDTO.Remarks = shipmentStatusRequestDTO.Remarks;
        requestAddShipmentTrackingDTO.ShipmentStatusId = shipment.ShipmentStatusId;
        await _shipmentService.CreateShipmentTracking(requestAddShipmentTrackingDTO);
        return _mapper.Map<ShipmentStatusResponseDTO>(shipment);

    }
    private async Task<List<OrderItems>> UpdateOrderItemStatus(int shipmentId, int orderItemStatus)
    {
        List<OrderItems> orderList = new List<OrderItems>();
        var orderItem = await _shipmentItemsRepsository.GetOrderItemsByShippingId(shipmentId);
        if (orderItem.Count == 0)
        {
            throw new DataNotFoundException("Order Item Not Found");
        }
        foreach (var order in orderItem)
        {
            order.OrderItemStatusId = orderItemStatus;
            await _orderItemRepsository.Update(order.OrderItemsId, order);
            orderList.Add(order);
        }
        await CheckIfAllOrderItemsShipped(orderItem[0].OrderId);
        return orderList;

    }
    private async Task<Order?> CheckIfAllOrderItemsShipped(int orderId)
    {
        var shipment = await _shipmentItemsRepsository.GetAllNotDeliveredOrderByOrderId(orderId);
        if (shipment.Count == 0)
        {
            var order = await _orderRepsository.Get(orderId);
            if (order == null)
            {
                throw new DataNotFoundException("Order Not Found");
            }
            order.OrderStatusId = 3;
            await _orderRepsository.Update(orderId, order);
            return order;
        }
        return null;
    }
}