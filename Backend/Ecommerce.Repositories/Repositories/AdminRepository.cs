using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class AdminRepsository : AbstractRepository<int, AdminUser> ,IAdminUserRepsository
{
    public AdminRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}