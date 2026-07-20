using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class ShiprocketService : IShipRocketService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ShiprocketService> _logger;

    public ShiprocketService(HttpClient httpClient, IConfiguration configuration, ILogger<ShiprocketService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<ShiprocketLoginResponseDTO?> GenerateToken(ShiprocketLoginRequestDTO request)
    {
        _logger.LogInformation("Generating Shiprocket authentication token.");

        var response = await _httpClient.PostAsJsonAsync("https://apiv2.shiprocket.in/v1/external/auth/login",
        new
        {
            email = request.Email,
            password = request.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Shiprocket authentication failed with StatusCode: {StatusCode}", response.StatusCode);
            throw new Exception("Shiprocket login failed");
        }

        _logger.LogInformation("Shiprocket authentication successful.");

        var result = await response.Content.ReadFromJsonAsync<ShiprocketLoginResponseDTO>();
        return result;
    }

    public async Task<CourierEstimateResponseDTO?> CheckServiceability(ServiceabilityRequestDTO request)
    {
        _logger.LogInformation("Checking courier serviceability from {PickupPostcode} to {DeliveryPostcode} with Weight: {Weight}", request.PickupPostcode, request.DeliveryPostcode, request.Weight);

        string token = await GetShiprocketToken();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        string url = "https://apiv2.shiprocket.in/v1/external/courier/serviceability/" +
                     $"?pickup_postcode={request.PickupPostcode}" +
                     $"&delivery_postcode={request.DeliveryPostcode}" +
                     $"&weight={request.Weight}" +
                     $"&cod={request.Cod}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Shiprocket serviceability check failed with StatusCode: {StatusCode}", response.StatusCode);
            throw new Exception("Serviceability check failed");
        }

        _logger.LogInformation("Shiprocket serviceability response received successfully.");

        var json = await response.Content.ReadAsStringAsync();

        using JsonDocument document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("data", out var data))
        {
            _logger.LogWarning("Shiprocket response does not contain data.");
            throw new Exception("Shiprocket response does not contain data");
        }

        if (!data.TryGetProperty("available_courier_companies", out var courierCompanies))
        {
            _logger.LogWarning("No courier companies available for PickupPostcode: {PickupPostcode}, DeliveryPostcode: {DeliveryPostcode}", request.PickupPostcode, request.DeliveryPostcode);
            throw new Exception("No courier companies available for this request");
        }

        List<CourierEstimateResponseDTO> result = new();

        foreach (var courier in courierCompanies.EnumerateArray())
        {
            CourierEstimateResponseDTO dto = new()
            {
                CourierName = courier.GetProperty("courier_name").GetString() ?? string.Empty,
                Rate = courier.GetProperty("rate").GetDecimal(),
                EstimatedDeliveryDays = courier.GetProperty("estimated_delivery_days").GetString() ?? string.Empty,
                ExpectedDeliveryDate = courier.GetProperty("etd").GetString() ?? string.Empty,
                Rating = courier.GetProperty("rating").GetDouble()
            };

            result.Add(dto);
        }

        _logger.LogInformation("Retrieved {CourierCount} courier options from Shiprocket.", result.Count);

        var bestCourier = GetBestCourier(result);

        if (bestCourier != null)
        {
            _logger.LogInformation("Selected courier {CourierName} with Rate: {Rate} and DeliveryDays: {DeliveryDays}", bestCourier.CourierName, bestCourier.Rate, bestCourier.EstimatedDeliveryDays);
        }
        else
        {
            _logger.LogWarning("No suitable courier found after applying selection criteria.");
        }

        return bestCourier;
    }

    private CourierEstimateResponseDTO? GetBestCourier(List<CourierEstimateResponseDTO> couriers)
    {
        _logger.LogInformation("Selecting the best courier from {CourierCount} available couriers.", couriers.Count);

        return couriers
            .Where(c => c.Rating >= 4)
            .OrderBy(c => int.Parse(c.EstimatedDeliveryDays))
            .ThenBy(c => c.Rate)
            .FirstOrDefault();
    }

    private async Task<string> GetShiprocketToken()
    {
        _logger.LogInformation("Requesting Shiprocket token.");

        var loginResponse = await GenerateToken(new ShiprocketLoginRequestDTO
        {
            Email = _configuration["Shiprocket:Email"] ?? string.Empty,
            Password = _configuration["Shiprocket:Password"] ?? string.Empty
        });

        _logger.LogInformation("Shiprocket token generated successfully.");

        return loginResponse?.Token ?? throw new Exception("Unable to generate Shiprocket token");
    }
}