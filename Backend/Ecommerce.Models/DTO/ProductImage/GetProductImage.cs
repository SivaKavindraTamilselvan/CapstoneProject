namespace Ecommerce.DTOs;
public class ResponseProductImageDTO
{
    public int ProductImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMainImage { get; set; }
    public string DisplayOrderName { get; set; } = string.Empty;
}