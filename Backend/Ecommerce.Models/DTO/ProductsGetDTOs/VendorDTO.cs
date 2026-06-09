// For vendor viewing all their products
public class ResponseVendorGetAllProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductApprovalStatus { get; set; } = string.Empty;
    public string ProductStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailableForSale { get; set; }
    public List<string> ValidationIssues { get; set; } = new();
    public List<ResponseVendorProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

public class ResponseVendorProductVariantDTO
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public int MinimumQuantityPerUser { get; set; }
    public string ProductApprovalStatus { get; set; } = string.Empty;
    public string ProductVariantStatus { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ResponseProductVariantAttributeDTO> Attributes { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

// For vendor draft products
public class ResponseVendorGetDraftProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ResponseVendorProductVariantDTO> ProductVariants { get; set; } = new();
}

// For vendor low stock / out of stock
public class ResponseVendorGetStockProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public List<ResponseVendorStockVariantDTO> ProductVariants { get; set; } = new();
}

public class ResponseVendorStockVariantDTO
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public List<ResponseProductVariantAttributeDTO> Attributes { get; set; } = new();
}