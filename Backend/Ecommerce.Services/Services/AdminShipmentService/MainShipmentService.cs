using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminShipmentService : IAdminShipmentService
{
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AdminShipmentService> _logger;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogChanges _logChanges;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IShipmentService _shipmentService;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IMapper _mapper;
    public AdminShipmentService(IInventoryRepsository inventoryRepsository,IAdminUserRepsository adminUserRepsository,INotificationService notificationService,ILogger<AdminShipmentService> logger,IAdminUserValidation adminUserValidation,EcommerceContext ecommerceContext,ILogChanges logChanges,IReturnRepsository returnRepsository,IShipmentService shipmentService, IShipmentRepsository shipmentRepsository, IMapper mapper, IShipmentItemsRepsository shipmentItemsRepsository, IOrderItemRepsository orderItemRepsository, IOrderRepsository orderRepsository)
    {
        _inventoryRepsository = inventoryRepsository;
        _adminUserRepsository = adminUserRepsository;
        _notificationService = notificationService;
        _logger = logger;
        _adminUserValidation = adminUserValidation;
        _logChanges = logChanges;
        _ecommerceContext = ecommerceContext;
        _returnRepsository = returnRepsository;
        _shipmentService = shipmentService;
        _shipmentRepsository = shipmentRepsository;
        _shipmentItemsRepsository = shipmentItemsRepsository;
        _orderItemRepsository = orderItemRepsository;
        _orderRepsository = orderRepsository;
        _mapper = mapper;
    }
}