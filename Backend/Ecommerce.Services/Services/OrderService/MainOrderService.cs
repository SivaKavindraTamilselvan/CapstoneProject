using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class OrderService : IOrderService
{
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly ILogChanges _logChanges;
    private readonly ILogger<OrderService> _logger;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IProductImageRepsository _productImageRepsository;
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
    public OrderService(IShipmentItemsRepsository shipmentItemsRepsository,IProductVariantRepsository productVariantRepsository,ILogChanges logChanges,ILogger<OrderService> logger,IReturnRepsository returnRepsository,IProductImageRepsository productImageRepsository,ICouponUsageRepsository couponUsageRepsository,IVendorUserValidation vendorUserValidation,IUserRepsository userRepsository,IShipmentService shipmentService, IShipmentRepsository shipmentRepsository, IInventoryRepsository inventoryRepsository, IInventoryValidation inventoryValidation, IInventoryService inventoryService, IOrderRepsository orderRepsository, IMapper mapper, IOrderItemRepsository orderItemRepsository)
    {
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _productVariantRepsository = productVariantRepsository;
        _logChanges = logChanges;
        _logger = logger;
        _returnRepsository = returnRepsository;
        _productImageRepsository = productImageRepsository;
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