namespace Ecommerce.DTOs;

public class RequestAdminProductVariantFilter : PaginationFilter
{
    public int? VendorId { get; set; }
    public string? SKU { get; set; }
    public int? ProductId { get; set; }
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? StatusId { get; set; }
    public int? ApprovalStatusId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinimuQuantityPerUser { get; set; }
    public int? AddedByVendorUserId { get; set; }
    public bool? IsReturn { get; set; } = true;
    public bool? IsExchange { get; set; } = true;
    public int? MainProductSubCategoryAttributeId { get; set; }
    public int? MinAvailableQuantity { get; set; }
    public int? MinReservedQuantity { get; set; }
    public int? MaxAvailableQuantity { get; set; }
    public int? MaxReservedQuantity { get; set; }
    public bool? hasIssues { get; set; }
    public bool? IsAvailableForSale { get; set; }
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
    public bool IsReturn { get; set; } = true;
    public bool IsExchange { get; set; } = true;
    public bool? IsAvailableForSale { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> ValidationIssues { get; set; } = new List<string>();
    public List<ResponseProductVariantAttribute> Attributes { get; set; } = new List<ResponseProductVariantAttribute>();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new List<ResponseProductImageDTO>();
}


public class ResponseAdminProductVariantOnlyDTO
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public string ProductCategoryName { get; set; } = string.Empty;
    public string ProductApprovalStatus { get; set; } = string.Empty;
    public string ProductStatus { get; set; } = string.Empty;
    public string MainProductSubCategoryAttributeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public int MinimumQuantityPerUser { get; set; }
    public string ProductVariantApprovalStatus { get; set; } = string.Empty;
    public string ProductVariantStatus { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public bool IsReturn { get; set; } = true;
    public bool IsExchange { get; set; } = true;
    public bool? IsAvailableForSale { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> ValidationIssues { get; set; } = new List<string>();
    public List<ResponseProductVariantAttribute> Attributes { get; set; } = new List<ResponseProductVariantAttribute>();
    public List<ResponseProductImageDTO> ProductImages { get; set; } = new List<ResponseProductImageDTO>();
}
