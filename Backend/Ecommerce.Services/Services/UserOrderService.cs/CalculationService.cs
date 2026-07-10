using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserOrderService : IUserOrderService
{
    private async Task<decimal> CalculateShippingCharge(Address pickup, Address delivery, decimal weight, int cod)
    {
        ServiceabilityRequestDTO serviceabilityRequestDTO = new ServiceabilityRequestDTO();
        serviceabilityRequestDTO.Cod = cod;
        serviceabilityRequestDTO.DeliveryPostcode = delivery.PinCode;
        serviceabilityRequestDTO.Weight = weight;
        serviceabilityRequestDTO.PickupPostcode = pickup.PinCode;
        var bestCourier = await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
        if (bestCourier == null)
        {
            throw new Exception("No courier available");
        }
        return bestCourier.Rate;

    }
    private decimal CalculateProductCharge(List<CartItems> cartItems)
    {
        decimal productCharge = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        return productCharge;
    }

}