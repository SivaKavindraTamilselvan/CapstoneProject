using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class OrderService : IOrderService
{
    private readonly IUserRepsository _userRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly ICouponUsageRepsository _couponUsageRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentService _shipmentService;

    private readonly IMapper _mapper;
    public OrderService(ICouponUsageRepsository couponUsageRepsository,IVendorUserValidation vendorUserValidation,IUserRepsository userRepsository,IShipmentService shipmentService, IShipmentRepsository shipmentRepsository, IInventoryRepsository inventoryRepsository, IInventoryValidation inventoryValidation, IInventoryService inventoryService, IOrderRepsository orderRepsository, IMapper mapper, IOrderItemRepsository orderItemRepsository)
    {
        _couponUsageRepsository = couponUsageRepsository;
        _userRepsository = userRepsository;
        _vendorUserValidation = vendorUserValidation;
        _orderRepsository = orderRepsository;
        _orderItemRepsository = orderItemRepsository;
        _inventoryValidation = inventoryValidation;
        _inventoryRepsository = inventoryRepsository;
        _shipmentService = shipmentService;
        _shipmentRepsository = shipmentRepsository;
        _mapper = mapper;
    }
}