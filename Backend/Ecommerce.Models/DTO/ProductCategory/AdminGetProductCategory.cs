namespace Ecommerce.DTOs;

public class RequestProductCategoryFilter : PaginationFilter
{
    public int? ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public bool? status { get; set; }
    public int? AddedByAdminId { get; set; }
}

public class ResponseAdminGetAllCategory
{
    public int ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public bool IsActive { get; set; } = true;
    public int AddedByAdminId { get; set; }
    public string? AddedUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class RequestProductSubCategoryFilter : PaginationFilter
{
    public int? ProductSubCategoryId { get; set; }
    public int? ProductCategoryId { get; set; }
    public bool? status { get; set; }
    public decimal? MinimumCommissionPercentage { get; set; }
    public decimal? MaximumCommissionPercentage { get; set; }
    public int? AddedByAdminId { get; set; }
}
public class ResponseAdminGetAllSubCategory
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
    public bool CategoryIsActive { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public decimal CommissionPercentage { get; set; }
    public int AddedByAdminId { get; set; }
    public string? AddedUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}
