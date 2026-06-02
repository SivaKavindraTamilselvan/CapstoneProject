using System.ComponentModel.DataAnnotations;

public class RequestAddFavoriteItemsDTO
{
    [Required(ErrorMessage = "Product Variant Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id Should Be Greater Than 0")]
    public int ProductVariantId {get;set;}
}
public class ResponseAddFavoriteItemsDTO
{
    public int FavoritesItemsId {get;set;}
    public int FavoritesId {get;set;}
    public int ProductVariantId {get;set;}
}

