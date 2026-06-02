using System.ComponentModel.DataAnnotations;

public class RequestAddCartItemsDTO
{
    [Required(ErrorMessage = "Product Variant Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id Should Be Greater Than 0")]
    public int ProductVariantId {get;set;}
}
public class ResponseAddCartItemsDTO
{
    public int CartItemsId {get;set;}
    public int CartId {get;set;}
    public int ProductVariantId {get;set;}
    public int Qunatity {get;set;}
}

