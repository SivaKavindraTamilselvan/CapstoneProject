using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminReturnService : IAdminReturnService
{
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IMapper _mapper;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IShipRocketService _shipRocketService;
    private readonly IShipmentService _shipmentService;
    public AdminReturnService(IShipmentRepsository shipmentRepsository, IMapper mapper, IInventoryValidation inventoryValidation, IOrderValidation orderValidation, IReturnRepsository returnRepsository, IShipRocketService shipRocketService, IShipmentService shipmentService)
    {
        _shipmentRepsository = shipmentRepsository;
        _mapper = mapper;
        _inventoryValidation = inventoryValidation;
        _orderValidation = orderValidation;
        _returnRepsository = returnRepsository;
        _shipRocketService = shipRocketService;
        _shipmentService = shipmentService;
    }
    public async Task<ResponseCreateReturnShipmentDTO> CreateReturnShipment(int returnId)
    {
        var user = await _returnRepsository.GetTheReturnUserByReturnId(returnId);
        if (user == null)
            throw new DataNotFoundException("Return User Not Found");

        var inventory = await _returnRepsository.GetTheReturnInventoryByReturnId(returnId);
        if (inventory == null)
            throw new DataNotFoundException("Return Inventory Not Found");

        var product = await _returnRepsository.GetTheReturnProductByReturnId(returnId);
        if (product == null)
            throw new DataNotFoundException("Return Product Not Found");

        if (user.OrderItems == null)
            throw new DataNotFoundException("Return Order Item Not Found");

        if (user.OrderItems.Order == null)
            throw new DataNotFoundException("Order Not Found");

        if (user.OrderItems.Order.Address == null)
            throw new DataNotFoundException("Customer Address Not Found");

        if (product.OrderItems == null)
            throw new DataNotFoundException("Product Order Item Not Found");

        if (product.OrderItems.ProductVariant == null)
            throw new DataNotFoundException("Product Variant Not Found");

        if (inventory.OrderItems == null)
            throw new DataNotFoundException("Inventory Order Item Not Found");

        if (inventory.OrderItems.Inventory == null)
            throw new DataNotFoundException("Inventory Not Found");

        if (inventory.OrderItems.Inventory.Address == null)
            throw new DataNotFoundException("Inventory Address Not Found");

        var service = await CheckReturnShipmentServiceability(user, inventory, product);
        if (service == null)
        {
            throw new Exception("No Service Is Found");
        }
        var shipment = await CreateReturnShipmentRecord(user, service);

        await CreateReturnShipmentItem(shipment.ShipmentId, user.OrderItemId);
        var a = await _shipmentRepsository.Get(shipment.ShipmentId);
        shipment.TrackingNumber = $"SHIPTRACK-RETURN" + shipment.ShipmentId.ToString();
        shipment.ShipmentStatusId = 4;
        await _shipmentRepsository.Update(shipment.ShipmentId, shipment);
        var trackingRemarks = await CreateReturnShipmentTracking(shipment.ShipmentId, returnId);
        await UpdateInventory(inventory.OrderItems.InventoryId, user.ReturnQuantity);

        return new ResponseCreateReturnShipmentDTO
        {
            ShipmentId = shipment.ShipmentId,
            OrderId = user.OrderItems.Order.OrderId,
            ShippingCharge = service.Rate,
            ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
            TrackingRemarks = trackingRemarks
        };
    }
    private async Task<CourierEstimateResponseDTO?> CheckReturnShipmentServiceability(Return user, Return inventory, Return product)
    {
        var customerAddress = user.OrderItems!.Order!.Address!;
        var inventoryAddress = inventory.OrderItems!.Inventory!.Address!;
        var productVariant = product.OrderItems!.ProductVariant!;

        var serviceabilityRequestDTO = new ServiceabilityRequestDTO
        {
            Cod = 0,
            Weight = productVariant.WeightInKgs,
            DeliveryPostcode = inventoryAddress.PinCode,
            PickupPostcode = customerAddress.PinCode
        };

        return await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
    }
    private async Task<Shipment> CreateReturnShipmentRecord(Return user, CourierEstimateResponseDTO service)
    {
        var order = user.OrderItems!.Order!;

        double estimatedDays = Convert.ToDouble(service.EstimatedDeliveryDays);

        var requestAddShipmentDTO = new RequestAddShipmentDTO
        {
            ShipmentTypeId = 2,
            CourierName = service.CourierName,
            OrderId = order.OrderId,
            ExpectedDeliveryDate = DateTime.Now.AddDays(estimatedDays),
            ShippingCharge = service.Rate,
            PickupAddressId = order.AddressId
        };

        return await _shipmentService.CreateShipment(requestAddShipmentDTO);
    }
    private async Task CreateReturnShipmentItem(int shipmentId, int orderItemId)
    {
        await _shipmentService.CreateShipmentItems(shipmentId, orderItemId);
    }
    private async Task<string> CreateReturnShipmentTracking(int shipmentId, int returnId)
    {
        //var shipment = await _shipmentRepsository.Get(shipmentId);
        //Console.WriteLine("fsefs"+shipmentId);

        //shipment.TrackingNumber = $"SHIPTRACK-RETURN" + shipment.ShipmentId.ToString();
        //await _shipmentRepsository.Update(shipmentId,shipment);
        var orderUser = await _returnRepsository.GetTheReturnUserByReturnId(returnId);
        var customerAddress = orderUser!.OrderItems!.Order!.Address!;

        var remarks = "Return shipment created from customer location";

        var trackingDTO = new RequestAddShipmentTrackingDTO
        {
            ShipmentId = shipmentId,
            ShipmentStatusId = 4,
            Location = customerAddress.City,
            Remarks = remarks
        };

        await _shipmentService.CreateShipmentTracking(trackingDTO);

        return remarks;
    }

    private async Task UpdateInventory(int inventoryId, int Quantity)
    {
        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        inventory.AvailableQuantity = inventory.AvailableQuantity + Quantity;
        inventory.ReservedQuantity = inventory.ReservedQuantity - Quantity;
        inventory.UpdatedAt = DateTime.Now;
    }
    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForAdmin(RequestAdminReturnFilter request)
    {
        var result = await _returnRepsository.GetAllReturnsForAdmin(request);
        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.totalCount
        };
    }

    public async Task<ReturnSummaryDto> GetAllReturns(int returnId)
    {
        var result = await _returnRepsository.GetReturnSummaryById(returnId);
        return _mapper.Map<ReturnSummaryDto>(result);

    }

}