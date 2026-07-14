using System.ComponentModel.DataAnnotations;
namespace Ecommerce.DTOs;

public class RequestDeleteProductDTO
{
    [Required(ErrorMessage = "Product Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id must be greater than 0.")]
    public int ProductId { get; set; }

    public string Remarks{get;set;} = string.Empty;
}

public class RequestDeleteVariantDTO
{
    [Required(ErrorMessage = "Product Variant Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id must be greater than 0.")]
    public int ProductVariantId { get; set; }

    public string Remarks{get;set;} = string.Empty;
}

public class ResponseDeleteProduct
{
    public int ProductId {get;set;}
    public int ProductApprovalStatusId { get; set; }
    public DateTime? UpdatedAt{get;set;}

}