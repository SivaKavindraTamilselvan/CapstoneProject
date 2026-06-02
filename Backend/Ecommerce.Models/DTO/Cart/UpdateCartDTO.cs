using System.ComponentModel.DataAnnotations;

public class RequestUpdateCartItemsDTO
{
    [Required(ErrorMessage = "Product Variant Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id Should Be Greater Than 0")]
    public int CartItemsId {get;set;}
    public int Qunatity {get;set;}
}


