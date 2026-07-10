namespace Ecommerce.DTOs;

public class RequestAdminProductFilter : PaginationFilter
{
    public int? VendorId { get; set; }
    public string? ProductName { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductSubCategoryId { get; set; }
    public int? ProductApprovalStatusId { get; set; }
    public int? ProductStatusId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public bool? hasIssues { get; set; }
    public bool? isAvailableForSale { get; set; }
    public int? MinAvailableQuantity { get; set; }
    public int? MinReservedQuantity { get; set; }
    public int? MaxAvailableQuantity { get; set; }
    public int? MaxReservedQuantity { get; set; }
    public int? MainProductSubCategoryAttributeId { get; set; }
}
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
    public string MainProductSubCategoryAttributeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailableForSale { get; set; }
    public List<string> ValidationIssues { get; set; } = new List<string>();
    public List<ResponseAdminProductVariantDTO> ProductVariants { get; set; } = new();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new();
}
public class ProductValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
}

