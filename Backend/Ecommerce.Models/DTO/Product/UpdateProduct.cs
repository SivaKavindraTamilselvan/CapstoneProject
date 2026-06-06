using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestUpdateProduct
{
    public int ProductId {get;set;}
    [Required(ErrorMessage = "Product Name Is Needed To Add The Product")]
    [MaxLength(100, ErrorMessage = "Maximum 100 characters allowed")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product Description Is Needed To Add The Product")]
    [MaxLength(1000, ErrorMessage = "Maximum 1000 characters allowed")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product Sub Category Id Needed To Add The Product")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Sub Category Id must be greater than 0")]
    public int ProductSubCategoryId { get; set; }
}
public class ResponseUpdateProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductSubCategoryId { get; set; }
    public DateTime UpdatedAt {get;set;}
    public DateTime CreatedAt { get; set; }
}
