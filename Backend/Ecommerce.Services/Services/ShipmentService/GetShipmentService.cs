using Ecommerce.DTOs;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class ShipmentService : IShipmentService
{
    public async Task<PagedResponse<ShipmentDetailResponseDto>> GetAllShipmentsForAdmin(RequestShipmentFilter filter)
    {
        _logger.LogInformation("Getting shipments for admin. PageNumber: {PageNumber}, PageSize: {PageSize}", filter.PageNumber, filter.PageSize);

        var result = await _shipmentRepsository.GetAllShipmentsForAdmin(filter);

        _logger.LogInformation("Retrieved {ShipmentCount} shipments out of {TotalCount} total shipments", result.Items.Count, result.TotalCount);

        return new PagedResponse<ShipmentDetailResponseDto>
        {
            Items = _mapper.Map<List<ShipmentDetailResponseDto>>(result.Items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = result.TotalCount,
        };
    }

    public async Task<ShipmentDetailResponseDto> GetShipmentDetailForAdmin(int shipmentId)
    {
        _logger.LogInformation("Getting shipment details for ShipmentId: {ShipmentId}", shipmentId);

        var shipment = await _shipmentRepsository.GetShipmentDetailForAdmin(shipmentId);

        if (shipment == null)
        {
            _logger.LogWarning("Shipment not found. ShipmentId: {ShipmentId}", shipmentId);
            throw new DataNotFoundException("Shipment not found");
        }

        _logger.LogInformation("Retrieved shipment details for ShipmentId: {ShipmentId}", shipmentId);

        return _mapper.Map<ShipmentDetailResponseDto>(shipment);
    }

    public async Task<ShipmentDetailResponseDto> GetShipmentDetailForOrderItemId(int orderItemId)
    {
        _logger.LogInformation("Getting shipment details for OrderItemId: {OrderItemId}", orderItemId);

        var shipment = await _shipmentRepsository.GetShipmentDetailForOrderItemsId(orderItemId);

        if (shipment == null)
        {
            _logger.LogWarning("Shipment not found for OrderItemId: {OrderItemId}", orderItemId);
            throw new DataNotFoundException("Shipment not found");
        }

        _logger.LogInformation("Retrieved shipment details for OrderItemId: {OrderItemId}", orderItemId);

        return _mapper.Map<ShipmentDetailResponseDto>(shipment);
    }
}