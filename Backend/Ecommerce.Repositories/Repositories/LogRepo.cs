using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class LogChangeRepsository : AbstractRepository<int, LogChanges>, ILogChanges
{
    public LogChangeRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
}