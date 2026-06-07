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