using System.ComponentModel.DataAnnotations;

public class RequestDeleteFavoriteItemsDTO
{
    [Required(ErrorMessage = "Product Variant Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Variant Id Should Be Greater Than 0")]
    public int FavoritesItemsId {get;set;}
}


