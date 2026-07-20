using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    private readonly EcommerceContext _ecommerceContext;
    private readonly INotificationService _notificationService;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    private readonly ILogChanges _logChanges;
    private readonly IVendorValidation _vendorValidation;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductValidation _productValidation;
    private readonly IUserValidation _userValidation;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly ILogger<InventoryService> _logger;
    private readonly IMapper _mapper;
    public InventoryService(EcommerceContext ecommerceContext,INotificationService notificationService,ILogChanges logChanges,IVendorUserRepsository vendorUserRepsository,IVendorValidation vendorValidation,ILogger<InventoryService> logger,IVendorUserValidation vendorUserValidation,IProductValidation productValidation, IMapper mapper, IInventoryRepsository inventoryRepsository, IUserValidation userValidation, IInventoryValidation inventoryValidation)
    {
        _ecommerceContext = ecommerceContext;
        _notificationService = notificationService;
        _vendorUserRepsository = vendorUserRepsository;
        _logChanges = logChanges;
        _vendorValidation = vendorValidation;
        _vendorUserValidation = vendorUserValidation;
        _productValidation = productValidation;
        _inventoryRepsository = inventoryRepsository;
        _userValidation = userValidation;
        _inventoryValidation = inventoryValidation;
        _mapper = mapper;
        _logger = logger;

    }
}