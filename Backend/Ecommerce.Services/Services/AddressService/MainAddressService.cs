using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly ILogger<AddressService> _logger;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IUserValidation _userValidation;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    public AddressService(INotificationService notificationService,IVendorUserValidation vendorUserValidation, IOrderItemRepsository orderItemRepsository, IOrderRepsository orderRepsository, IAddressRepsository addressRepsository, IUserValidation userValidation, IMapper mapper, ILogger<AddressService> logger)
    {
        _notificationService = notificationService;
        _vendorUserValidation = vendorUserValidation;
        _orderItemRepsository = orderItemRepsository;
        _orderRepsository = orderRepsository;
        _addressRepsository = addressRepsository;
        _userValidation = userValidation;
        _mapper = mapper;
        _logger = logger;
    }
}