namespace Ecommerce.DTOs;

public class RequestAddProduct
{
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductSubCategoryId { get; set; }
}

public class RequestAddProductImage
{
    public int ProductId {get;set;}
    public int? ProductVariantId {get;set;}
    public string ImageUrl {get;set;} = string.Empty;
    public int DisplayOrderId {get;set;}
    public bool IsMainImage {get;set;} = false;
}
public class ResponseAddProduct
{
    public int ProductId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}

public class ResponseAddProductImage
{
    public int ProductImageId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}