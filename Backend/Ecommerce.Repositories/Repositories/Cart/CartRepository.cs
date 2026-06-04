using System.Security.Cryptography.X509Certificates;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CartRepsository : AbstractRepository<int, Cart>, ICartRepsository
{
    public CartRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}