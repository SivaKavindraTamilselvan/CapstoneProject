namespace Ecommerce.DTOs;



public class ResponseUserGetAllCategory
{
    public int ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
}
public class ResponseUserGetAllSubCategory
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public bool IsActive {get;set;}
    public DateTime CreatedAt{get;set;}
}
public class ResponseVendorGetAllProductSubCategory
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public decimal CommissionPercentage { get; set; }
}
public class ResponseGetAllProductSubCategoryAttribute
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
}