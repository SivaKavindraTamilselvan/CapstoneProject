namespace Ecommerce.DTOs;
public class ResponseGetAllProduct()
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public string VendorName {get;set;} = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategory {get;set;} = string.Empty;
}

public class ResponseGetAllProductByProductId()
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public string VendorName {get;set;} = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategory {get;set;} = string.Empty;

    public List<ResponseGetAllProductVariant> responseGetAllProductVariants {get;set;} = new List<ResponseGetAllProductVariant>();

}

public class RequestGetAllProductByProductId()
{
    public int ProductId { get; set; }
}

public class ProductValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class ProductVariantFilterDto
{
    public string? SearchTerm      { get; set; }   // matches SKU or Product name
    public int?    CategoryId      { get; set; }
    public int?    SubCategoryId   { get; set; }
    public int?    StatusId        { get; set; }   // ProductVariantStatusId
    public int?    ApprovalStatusId{ get; set; }
    public decimal? MinPrice       { get; set; }
    public decimal? MaxPrice       { get; set; }
    public int     Page            { get; set; } = 1;
    public int     PageSize        { get; set; } = 10;
}