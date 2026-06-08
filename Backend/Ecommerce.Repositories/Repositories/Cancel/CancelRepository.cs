using System.Security.Cryptography.X509Certificates;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CancelRepsository : AbstractRepository<int, Cancel>, ICancelRepsository
{
    public CancelRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}