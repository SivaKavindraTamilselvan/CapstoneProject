using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminShipmentService
{
     public Task<ShipmentStatusResponseDTO> UpdateShimentStatus(ShipmentStatusRequestDTO shipmentStatusRequestDTO);
}