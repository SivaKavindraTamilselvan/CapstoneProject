using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddProductSubCategoryDTO
{
    [Required(ErrorMessage = "Attribute Name Is Needed")]
    [MaxLength(100 ,ErrorMessage = "Maximum 100 characters allowed")]
    public string ProductSubCategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product Category Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Category Id Should Be Greater Than 0")]
    public int ProductCategoryId { get; set; }

}
public class ResponseAddProductSubCategoryDTO
{
    public int ProductSubCategoryId { get; set; }
    public string ProductSubCategoryName { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
}
