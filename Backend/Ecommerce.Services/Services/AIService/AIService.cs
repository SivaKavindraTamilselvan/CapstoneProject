/*
using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.DTOs;
using Ecommerce.Models;
using System.Net.Http;

public class AiProductValidationService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _imageClient;

    public AiProductValidationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _imageClient = new HttpClient();
    }

    public async Task<AiValidationResult> ValidateVariantAsync(ResponseVendorGetProductVariantOnly variant)
    {
        var imageBase64List = new List<string>();

        foreach (var img in variant.ProductImages)
        {
            if (string.IsNullOrWhiteSpace(img.ImageUrl)) continue;

            var fullUrl = img.ImageUrl.StartsWith("http")
                ? img.ImageUrl
                : $"http://localhost:5173{img.ImageUrl}";

            try
            {
                var bytes = await _imageClient.GetByteArrayAsync(fullUrl);
                var base64 = Convert.ToBase64String(bytes);
                var mimeType = fullUrl.EndsWith(".png") ? "image/png" : "image/jpeg";
                imageBase64List.Add($"data:{mimeType};base64,{base64}");
            }
            catch
            {
                
            }
        }
        var payload = new
        {
            productName = variant.ProductName,
            description = variant.Description,
            productCategoryName = variant.ProductCategoryName,
            productSubCategoryName = variant.ProductSubCategoryName,
            sku = variant.SKU,
            attributes = variant.Attributes.Select(a => new
            {
                attributeName = a.AttributeName,
                attributeValue = a.AttributeValue
            }).ToList(),
            imageUrls = imageBase64List
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:8000/validate-variant",
            payload,
            new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AI service returned {response.StatusCode}: {errorBody}");
        }
        return await response.Content.ReadFromJsonAsync<AiValidationResult>();
    }

    public async Task<AiValidationResult> ValidateAsync(ResponseAdminGetAllProductDTO product)
    {
        var imageBase64List = new List<string>();

        foreach (var img in product.ProductImages)
        {
            if (string.IsNullOrWhiteSpace(img.ImageUrl)) continue;

            var fullUrl = img.ImageUrl.StartsWith("http")
                ? img.ImageUrl
                : $"http://localhost:5173/images/{img.ImageUrl}";

            try
            {
                var bytes = await _imageClient.GetByteArrayAsync(fullUrl);
                var base64 = Convert.ToBase64String(bytes);
                var mimeType = fullUrl.EndsWith(".png") ? "image/png" : "image/jpeg";
                imageBase64List.Add($"data:{mimeType};base64,{base64}");
            }
            catch
            {

            }
        }

        var payload = new
        {
            name = product.ProductName,
            description = product.Description,
            category = product.ProductCategoryName,
            subCategory = product.ProductSubCategoryName,
            imageUrls = imageBase64List 
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/validate-product", payload,
            new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AI service returned {response.StatusCode}: {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<AiValidationResult>();
    }
}

public class AiValidationResult
{
    public bool IsValid { get; set; }
    public int Confidence { get; set; }
    public List<string> Issues { get; set; }
    public string Recommendation { get; set; }
}

*/

using System.Net.Http.Json;
using System.Text.Json;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.Extensions.Configuration;

public class AiProductValidationService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _imageClient;
    private readonly string _frontendBaseUrl;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AiProductValidationService(
        HttpClient httpClient,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _imageClient = httpClientFactory.CreateClient(); 
        _frontendBaseUrl = configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
    }

    public async Task<AiValidationResult> ValidateVariantAsync(ResponseVendorGetProductVariantOnly variant)
    {
        var imageBase64List = new List<string>();

        foreach (var img in variant.ProductImages)
        {
            if (string.IsNullOrWhiteSpace(img.ImageUrl)) continue;

            var fullUrl = img.ImageUrl.StartsWith("http")
                ? img.ImageUrl
                : $"{_frontendBaseUrl}{img.ImageUrl}";

            try
            {
                var bytes = await _imageClient.GetByteArrayAsync(fullUrl);
                var base64 = Convert.ToBase64String(bytes);
                var mimeType = fullUrl.EndsWith(".png") ? "image/png" : "image/jpeg";
                imageBase64List.Add($"data:{mimeType};base64,{base64}");
            }
            catch
            {
                // image fetch failed, skip
            }
        }

        var payload = new
        {
            productName = variant.ProductName,
            description = variant.Description,
            productCategoryName = variant.ProductCategoryName,
            productSubCategoryName = variant.ProductSubCategoryName,
            sku = variant.SKU,
            attributes = variant.Attributes.Select(a => new
            {
                attributeName = a.AttributeName,
                attributeValue = a.AttributeValue
            }).ToList(),
            imageUrls = imageBase64List
        };

        // Uses _httpClient.BaseAddress (configured in Program.cs from Key Vault) instead of hardcoded localhost
        var response = await _httpClient.PostAsJsonAsync("/validate-variant", payload, JsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AI service returned {response.StatusCode}: {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<AiValidationResult>();
    }

    public async Task<AiValidationResult> ValidateAsync(ResponseAdminGetAllProductDTO product)
    {
        var imageBase64List = new List<string>();

        foreach (var img in product.ProductImages)
        {
            if (string.IsNullOrWhiteSpace(img.ImageUrl)) continue;

            var fullUrl = img.ImageUrl.StartsWith("http")
                ? img.ImageUrl
                : $"{_frontendBaseUrl}/images/{img.ImageUrl}";

            try
            {
                var bytes = await _imageClient.GetByteArrayAsync(fullUrl);
                var base64 = Convert.ToBase64String(bytes);
                var mimeType = fullUrl.EndsWith(".png") ? "image/png" : "image/jpeg";
                imageBase64List.Add($"data:{mimeType};base64,{base64}");
            }
            catch
            {
                // image fetch failed, skip
            }
        }

        var payload = new
        {
            name = product.ProductName,
            description = product.Description,
            category = product.ProductCategoryName,
            subCategory = product.ProductSubCategoryName,
            imageUrls = imageBase64List
        };

        var response = await _httpClient.PostAsJsonAsync("/validate-product", payload, JsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"AI service returned {response.StatusCode}: {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<AiValidationResult>();
    }
}

public class AiValidationResult
{
    public bool IsValid { get; set; }
    public int Confidence { get; set; }
    public List<string> Issues { get; set; }
    public string Recommendation { get; set; }
}