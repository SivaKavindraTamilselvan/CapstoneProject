using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IShipRocketService
{
    public Task<CourierEstimateResponseDTO?> CheckServiceability(ServiceabilityRequestDTO request);
}