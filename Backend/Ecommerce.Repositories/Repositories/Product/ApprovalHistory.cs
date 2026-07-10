using Ecommerce.Data;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public class ApprovalHistoryRepsository : AbstractRepository<int, ApprovalHistory> ,IApprovalHistoryRepsository
{
    public ApprovalHistoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}