using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class FavoriteRepsository : AbstractRepository<int, Favorites> ,IFavoriteRepsository
{
    public FavoriteRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}