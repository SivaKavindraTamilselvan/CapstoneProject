using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class CancelService : ICancelService
{
    private readonly IOrderService _orderService;
    private readonly IUserRepsository _userRepsository;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancelService> _logger;
    private readonly ILogChanges _logChanges;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly IOrderValidation _orderValidation;
    private readonly ICancelRepsository _cancelRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IRefundRepsository _refundRepsository;
    private readonly ICancelRefundRepsository _cancelRefundRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;
    public CancelService(IOrderService orderService,IUserRepsository userRepsository,IInventoryRepsository inventoryRepsository,IAdminUserRepsository adminUserRepsository,IVendorUserRepsository vendorUserRepsository,INotificationService notificationService,ILogger<CancelService> logger,ILogChanges logChanges,IVendorUserValidation vendorUserValidation,IOrderItemRepsository orderItemRepsository, IRefundRepsository refundRepsository, ICancelRefundRepsository cancelRefundRepsository, IOrderRepsository orderRepsository, IInventoryValidation inventoryValidation, ICancelRepsository cancelRepsository, IOrderValidation orderValidation, IMapper mapper)
    {
        _orderService = orderService;
        _userRepsository = userRepsository;
        _inventoryRepsository = inventoryRepsository;
        _adminUserRepsository = adminUserRepsository;
        _vendorUserRepsository = vendorUserRepsository;
        _notificationService = notificationService;
        _logChanges = logChanges;
        _logger = logger;
        _vendorUserValidation = vendorUserValidation;
        _orderItemRepsository = orderItemRepsository;
        _refundRepsository = refundRepsository;
        _orderRepsository = orderRepsository;
        _cancelRefundRepsository = cancelRefundRepsository;
        _inventoryValidation = inventoryValidation;
        _cancelRepsository = cancelRepsository;
        _orderValidation = orderValidation;
        _mapper = mapper;
    }
}