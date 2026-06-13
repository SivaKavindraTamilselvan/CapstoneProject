using Ecommerce.DTOs.Shipment;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class ShipmentService : IShipmentService
{
    public async Task<List<ShipmentSummaryResponseDto>> GetAllShipmentsForAdmin(ShipmentFilterDto filter)
    {
        var result = await _shipmentRepsository.GetAllShipmentsForAdmin(filter);


        return _mapper.Map<List<ShipmentSummaryResponseDto>>(result);
    }

    public async Task<ShipmentDetailResponseDto> GetShipmentDetailForAdmin(int shipmentId)
    {
        var shipment = await _shipmentRepsository.GetShipmentDetailForAdmin(shipmentId);

        if (shipment == null)
            throw new DataNotFoundException("Shipment not found");

        return _mapper.Map<ShipmentDetailResponseDto>(shipment);
    }
}