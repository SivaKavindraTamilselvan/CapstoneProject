using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductValidation _productValidation;
    private readonly IUserValidation _userValidation;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IInventoryValidation _inventoryValidation;
    private readonly ILogger<InventoryService> _logger;
    private readonly IMapper _mapper;
    public InventoryService(ILogger<InventoryService> logger,IVendorUserValidation vendorUserValidation,IProductValidation productValidation, IMapper mapper, IInventoryRepsository inventoryRepsository, IUserValidation userValidation, IInventoryValidation inventoryValidation)
    {
        _vendorUserValidation = vendorUserValidation;
        _productValidation = productValidation;
        _inventoryRepsository = inventoryRepsository;
        _userValidation = userValidation;
        _inventoryValidation = inventoryValidation;
        _mapper = mapper;
        _logger = logger;

    }
}