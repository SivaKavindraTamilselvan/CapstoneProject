// For available products listing (user browsing)
public class ResponseUserGetAllProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public List<ResponseUserProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

public class ResponseUserProductVariantDTO
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int MinimumQuantityPerUser { get; set; }
    public List<ResponseProductVariantAttributeDTO> Attributes { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

// For single product detail page
public class ResponseUserGetProductDetailDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductCategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public List<ResponseUserProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}