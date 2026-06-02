using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;
public class RequestAddProductImage
{
    [Required(ErrorMessage = "Product Id Needed To Add The Product Image")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id must be greater than 0")]
    public int ProductId {get;set;}

    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id must be greater than 0")]
    public int? ProductVariantId {get;set;}

    [Required(ErrorMessage = "Product Image URL Needed To Dsiplay The Product")]
    [MaxLength(1000, ErrorMessage = "Maximum 1000 characters allowed")]
    public string ImageUrl {get;set;} = string.Empty;

    [Required(ErrorMessage = "Product Id Needed To Add The Product Image")]
    [Range(1, 4, ErrorMessage = "Product Id must be greater than 0")]
    public int DisplayOrderId {get;set;}

    [Required(ErrorMessage = "Main Product Image Condition Is Needed")]
    public bool IsMainImage {get;set;} = false;
}

public class ResponseAddProductImage
{
    public int ProductImageId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}