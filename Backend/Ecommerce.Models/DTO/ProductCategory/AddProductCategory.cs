using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddProductCategoryDTO
{
    [Required(ErrorMessage = "Attribute Name Is Needed")]
    [MaxLength(100 ,ErrorMessage = "Maximum 100 characters allowed")]
    public string ProductCategoryName { get; set; } = string.Empty;
}


public class ResponseAddProductCategoryDTO
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
}
