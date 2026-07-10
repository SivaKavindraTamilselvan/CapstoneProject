using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class PaymentRepsository : AbstractRepository<int, Payment> ,IPaymentRepsository
{
    public PaymentRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}