namespace Ecommerce.DTOs;

public class RequestAddProductCategoryDTO
{
    public string ProductCategoryName { get; set; } = string.Empty;
}


public class ResponseAddProductCategoryDTO
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
}
