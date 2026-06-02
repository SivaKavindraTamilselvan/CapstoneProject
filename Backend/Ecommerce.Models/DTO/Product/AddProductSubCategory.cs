namespace Ecommerce.DTOs;

public class RequestAddProductSubCategoryDTO
{
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }

}
public class ResponseAddProductSubCategoryDTO
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
}
