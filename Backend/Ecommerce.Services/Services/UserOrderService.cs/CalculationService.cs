using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserOrderService : IUserOrderService
{
    private async Task<decimal> CalculateShippingCharge(Address pickup, Address delivery, decimal weight, int cod)
    {
        _logger.LogInformation("Calculating shipping charge. PickupPostcode {PickupPostcode}, DeliveryPostcode {DeliveryPostcode}, Weight {Weight}kg, Cod {Cod}",pickup.PinCode, delivery.PinCode, weight, cod);

        ServiceabilityRequestDTO serviceabilityRequestDTO = new ServiceabilityRequestDTO();
        serviceabilityRequestDTO.Cod = cod;
        serviceabilityRequestDTO.DeliveryPostcode = delivery.PinCode;
        serviceabilityRequestDTO.Weight = weight;
        serviceabilityRequestDTO.PickupPostcode = pickup.PinCode;

        var bestCourier = await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
        if (bestCourier == null)
        {
            _logger.LogWarning("No courier available for PickupPostcode {PickupPostcode} to DeliveryPostcode {DeliveryPostcode}, Weight {Weight}kg", pickup.PinCode, delivery.PinCode, weight);
            throw new Exception("No courier available");
        }

        _logger.LogInformation("Best courier {CourierName} found with Rate {Rate} for PickupPostcode {PickupPostcode} to DeliveryPostcode {DeliveryPostcode}",
            bestCourier.CourierName, bestCourier.Rate, pickup.PinCode, delivery.PinCode);

        return bestCourier.Rate;
    }

    private decimal CalculateProductCharge(List<CartItems> cartItems)
    {
        _logger.LogInformation("Calculating product charge for {ItemCount} cart items", cartItems.Count);

        decimal productCharge = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.Price);

        _logger.LogInformation("Calculated ProductCharge {ProductCharge} for {ItemCount} cart items", productCharge, cartItems.Count);

        return productCharge;
    }
}