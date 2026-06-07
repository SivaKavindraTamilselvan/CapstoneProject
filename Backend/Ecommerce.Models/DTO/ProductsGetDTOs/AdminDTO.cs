// For admin viewing all products (full details)
public class ResponseAdminGetAllProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductCategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string ProductApprovalStatus { get; set; } = string.Empty;
    public string ProductStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ResponseAdminProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

public class ResponseAdminProductVariantDTO
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

// For admin pending approval queue
public class ResponseAdminGetPendingProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ResponseAdminProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}

// For admin stock management
public class ResponseAdminGetStockProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public List<ResponseVendorStockVariantDTO> ProductVariants { get; set; } = new();
}