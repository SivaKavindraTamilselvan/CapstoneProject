namespace Ecommerce.DTOs;

public class ResponseGetAllProductCategory
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
}

public class RequestGetAllProductSubCategoryName
{
    public int ProductCategoryId { get; set; }
}

public class ResponseGetAllProductSubCategoryName
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
}


public class RequestGetAllProductSubCategoryNameVendor
{
    public int ProductSubCategoryId { get; set; }

}
public class ResponseGetAllProductSubCategoryNameVendor
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public decimal CommissionPercentage { get; set; }
}

public class RequestGetAllProductSubCategoryAttributeName
{
    public int ProductSubCategoryAttributeId { get; set; }
}

public class ResponseGetAllProductSubCategoryAttributeName
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
}

public class ResponseGetAllAttributeName
{
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
}
