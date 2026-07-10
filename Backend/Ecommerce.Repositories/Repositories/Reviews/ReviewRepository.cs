using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ReviewRepsository : AbstractRepository<int, Reviews> ,IReviewRepsository
{
    public ReviewRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}