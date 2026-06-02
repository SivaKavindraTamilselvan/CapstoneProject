using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class FavoriteItemsRepsository : AbstractRepository<int, FavoritesItems> ,IFavoriteItemsRepsository
{
    public FavoriteItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}