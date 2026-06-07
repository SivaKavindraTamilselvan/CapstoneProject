using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminReturnService
{
    public Task<ResponseCreateReturnShipmentDTO> CreateReturnShipment(int returnId);
}