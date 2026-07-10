using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;

public class ShiprocketService : IShipRocketService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ShiprocketService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<ShiprocketLoginResponseDTO?> GenerateToken(ShiprocketLoginRequestDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync("https://apiv2.shiprocket.in/v1/external/auth/login",
        new
        {
            email = request.Email,
            password = request.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Shiprocket login failed");
        }
        var result = await response.Content.ReadFromJsonAsync<ShiprocketLoginResponseDTO>();
        return result;
    }
    public async Task<CourierEstimateResponseDTO?> CheckServiceability(ServiceabilityRequestDTO request)
    {
        string token = await GetShiprocketToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        string url = "https://apiv2.shiprocket.in/v1/external/courier/serviceability/" + $"?pickup_postcode={request.PickupPostcode}" +
            $"&delivery_postcode={request.DeliveryPostcode}" + $"&weight={request.Weight}" + $"&cod={request.Cod}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Serviceability check failed");
        }

        var json = await response.Content.ReadAsStringAsync();

        using JsonDocument document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("data", out var data))
        {
            throw new Exception("Shiprocket response does not contain data");
        }

        if (!data.TryGetProperty("available_courier_companies", out var courierCompanies))
        {
            throw new Exception("No courier companies available for this request");
        }
        List<CourierEstimateResponseDTO> result = new();

        foreach (var courier in courierCompanies.EnumerateArray())
        {
            CourierEstimateResponseDTO dto = new CourierEstimateResponseDTO
            {
                CourierName = courier.GetProperty("courier_name").GetString() ?? string.Empty,
                Rate = courier.GetProperty("rate").GetDecimal(),
                EstimatedDeliveryDays = courier.GetProperty("estimated_delivery_days").GetString() ?? string.Empty,
                ExpectedDeliveryDate = courier.GetProperty("etd").GetString() ?? string.Empty,
                Rating = courier.GetProperty("rating").GetDouble()
            };

            result.Add(dto);
        }

        return GetBestCourier(result);
    }
    private CourierEstimateResponseDTO? GetBestCourier(List<CourierEstimateResponseDTO> couriers)
    {
        return couriers.Where(c => c.Rating >= 4).OrderBy(c => int.Parse(c.EstimatedDeliveryDays)).ThenBy(c => c.Rate).FirstOrDefault();
    }
    private async Task<string> GetShiprocketToken()
    {
        var loginResponse = await GenerateToken(new ShiprocketLoginRequestDTO
        {
            Email = _configuration["Shiprocket:Email"] ?? string.Empty,
            Password = _configuration["Shiprocket:Password"] ?? string.Empty
        });

        return loginResponse?.Token ?? throw new Exception("Unable to generate Shiprocket token");
    }
}