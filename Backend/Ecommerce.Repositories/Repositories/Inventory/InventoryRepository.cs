using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class InventoryRepsository : AbstractRepository<int, Inventory>, IInventoryRepsository
{
    public InventoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}