using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;
public class RequestAddProductVariantImage
{

    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id must be greater than 0")]
    public int ProductVariantId {get;set;}

    [Required(ErrorMessage = "Product Image URL Needed To Dsiplay The Product")]
    [MaxLength(1000, ErrorMessage = "Maximum 1000 characters allowed")]
    public string ImageUrl {get;set;} = string.Empty;

    [Required(ErrorMessage = "Product Id Needed To Add The Product Image")]
    [Range(1, 4, ErrorMessage = "Product Id must be greater than 0")]
    public int DisplayOrderId {get;set;}
}

public class ResponseAddProductVariantImage
{
    public int ProductImageId {get;set;}
    public int AddedByVendorUserId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}