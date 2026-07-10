namespace Ecommerce.DTOs;

public class ShiprocketLoginRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class ShiprocketLoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
}

public class ServiceabilityRequestDTO
{
    public string PickupPostcode { get; set; } = string.Empty;
    public string DeliveryPostcode { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int Cod { get; set; }
}

public class CourierEstimateResponseDTO
{
    public string CourierName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string EstimatedDeliveryDays { get; set; } = string.Empty;
    public string ExpectedDeliveryDate { get; set; } = string.Empty;
    public double Rating { get; set; }
}