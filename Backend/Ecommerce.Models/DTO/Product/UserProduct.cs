namespace Ecommerce.DTOs;

public class RequestUserProductFilter : PaginationFilter
{
    public string? ProductName { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductSubCategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
}
public class ResponseUserGetProductDetailDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductCategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string MainProductSubCategoryAttributeName { get; set; } = string.Empty;
    public List<ResponseUserProductVariant> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}
