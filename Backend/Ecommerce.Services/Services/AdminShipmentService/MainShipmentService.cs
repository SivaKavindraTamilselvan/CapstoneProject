using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminShipmentService : IAdminShipmentService
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
}