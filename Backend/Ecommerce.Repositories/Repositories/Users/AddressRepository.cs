using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class AddressRepsository : AbstractRepository<int, Address> ,IAddressRepsository
{
    public AddressRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}