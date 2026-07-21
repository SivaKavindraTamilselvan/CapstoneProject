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

    private static string GuessMimeTypeFromExtension(string url)
    {
        if (url.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return "image/png";
        if (url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)) return "image/webp";
        if (url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)) return "image/gif";
        if (url.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)) return "image/bmp";
        if (url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) return "image/jpeg";
        return "image/jpeg";
    }

    private async Task<string?> FetchImageAsDataUriAsync(string fullUrl)
    {
        try
        {
            using var response = await _imageClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);

            // Use the actual Content-Type the server reports for this image, rather than
            // guessing from the file extension. Guessing from extension alone was mislabeling
            // images (e.g. a ".jpg" URL that actually served WebP bytes), which Anthropic
            // correctly rejected with a media-type mismatch error. Only fall back to extension
            // guessing if the server didn't send a Content-Type at all.
            var mimeType = response.Content.Headers.ContentType?.MediaType;
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                mimeType = GuessMimeTypeFromExtension(fullUrl);
            }

            return $"data:{mimeType};base64,{base64}";
        }
        catch
        {
            // image fetch failed, skip
            return null;
        }
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

            var dataUri = await FetchImageAsDataUriAsync(fullUrl);
            if (dataUri != null)
            {
                imageBase64List.Add(dataUri);
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

            var dataUri = await FetchImageAsDataUriAsync(fullUrl);
            if (dataUri != null)
            {
                imageBase64List.Add(dataUri);
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