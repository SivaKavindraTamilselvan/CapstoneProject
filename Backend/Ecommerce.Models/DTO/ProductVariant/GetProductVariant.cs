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
    public bool? isAvailableForSale { get; set; }

}
public class ResponseGetAllProductVariant
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public int MinimuQuantityPerUser { get; set; }
    public bool IsAvailableForSale { get; set; }
    public List<string> ValidationIssues { get; set; } = new List<string>();
    public List<ResponseProductVariantAttribute> responseProductVariantAttributes { get; set; } = new List<ResponseProductVariantAttribute>();
}