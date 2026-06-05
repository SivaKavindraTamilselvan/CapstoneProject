
using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorOrderService : IVendorOrderService
{
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IMapper _mapper;
    private readonly IVendorValidation _vendorValidation;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    public VendorOrderService(IOrderItemRepsository orderItemRepsository, IMapper mapper, IVendorValidation vendorValidation, IShipmentRepsository shipmentRepsository,IShipmentItemsRepsository shipmentItemsRepsository)
    {
        _orderItemRepsository = orderItemRepsository;
        _mapper = mapper;
        _vendorValidation = vendorValidation;
        _shipmentRepsository = shipmentRepsository;
        _shipmentItemsRepsository = shipmentItemsRepsository;
    }
    public async Task<List<ResponseGetOrderItems>> GetAllTheActiveOrder(int vendorId)
    {
        var vendor = await _vendorValidation.ValidateVendorUser(vendorId);
        Console.WriteLine(vendor.VendorId);
        var allOrders = await _orderItemRepsository.GetOrderItemsByVendor(vendor.VendorId);
        return _mapper.Map<List<ResponseGetOrderItems>>(allOrders);
    }
    public async Task<ResponseGetOrderItems> UpdateTheOrderStatus(int orderItemId)
    {
        var order = await _orderItemRepsository.Get(orderItemId);
        order.OrderItemStatusId = 2;
        var updatedOrder = await _orderItemRepsository.Update(orderItemId, order);
        await CheckIfAllOrderForShipmetPacked(orderItemId);
        return _mapper.Map<ResponseGetOrderItems>(updatedOrder);
    }
    private async Task<bool> CheckIfAllOrderForShipmetPacked(int orderItemId)
    {
        var shipment = await _shipmentRepsository.GetShipmentByOrderItemId(orderItemId);
        Console.WriteLine(shipment.ShipmentId);
        var orderItems = await _shipmentItemsRepsository.GetPendingPackedTheShipmentItemsByShipmentId(shipment.ShipmentId);
        if (orderItems.Count == 0)
        {
            string shipmentTrackingNumber = $"SHPTRACKI-" + shipment.ShipmentId.ToString();
            shipment.ShipmentStatusId = 8;
            shipment.TrackingNumber = shipmentTrackingNumber;
            await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        }
        return true;
    }
}