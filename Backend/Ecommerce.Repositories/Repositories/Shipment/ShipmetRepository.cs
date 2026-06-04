using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ShipmentItemRepsository : AbstractRepository<int, ShipmentItems>, IShipmentItemsRepsository
{
    public ShipmentItemRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}