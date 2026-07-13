using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ApprovalHistoryRepsository : AbstractRepository<int, ApprovalHistory> ,IApprovalHistoryRepsository
{
    public ApprovalHistoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<List<ApprovalHistory>> GetProductApprovalHistory(int productId)
    {
        return await _ecommerceContext.ApprovalHistory.Include(p=>p.PreviousStatus).Include(p=>p.NewStatus).Where(p=>p.EntityType == "Product" && p.EntityId == productId).ToListAsync();
    }
    public async Task<List<ApprovalHistory>> GetProductVariantApprovalHistory(int variantId)
    {
        return await _ecommerceContext.ApprovalHistory.Include(p=>p.PreviousStatus).Include(p=>p.NewStatus).Where(p=>p.EntityType == "Product_Variant" && p.EntityId == variantId).ToListAsync();
    }
}