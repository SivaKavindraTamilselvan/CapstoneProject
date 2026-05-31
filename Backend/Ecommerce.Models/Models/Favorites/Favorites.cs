namespace Ecommerce.Models;

public class Favorites
{
    public int FavoritesId {get;set;}
    public int UserId {get;set;}
    public User? Users {get;set;}
    public ICollection<FavoritesItems> FavoritesItems {get;set;} = new List<FavoritesItems>();
}