using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class RequestUploadProductImage
{
    [Required(ErrorMessage = "Product Id Needed To Add The Product Image")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id must be greater than 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Image file is required")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "Display Order Id is needed")]
    [Range(1, 4, ErrorMessage = "Display Order Id must be between 1 and 4")]
    public int DisplayOrderId { get; set; }

    [Required(ErrorMessage = "Main Product Image Condition Is Needed")]
    public bool IsMainImage { get; set; } = false;
}



public class RequestUploadProductVariantImage
{
    [Required(ErrorMessage = "Product Id Needed To Add The Product Image")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id must be greater than 0")]
    public int ProductVariantId { get; set; }

    [Required(ErrorMessage = "Image file is required")]
    public IFormFile File { get; set; } = null!;

    [Required(ErrorMessage = "Display Order Id is needed")]
    [Range(1, 4, ErrorMessage = "Display Order Id must be between 1 and 4")]
    public int DisplayOrderId { get; set; }

}
