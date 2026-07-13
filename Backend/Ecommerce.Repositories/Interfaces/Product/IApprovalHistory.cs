using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IApprovalHistoryRepsository : IRepository<int, ApprovalHistory>
{
    public  Task<List<ApprovalHistory>> GetProductApprovalHistory(int productId);
    public  Task<List<ApprovalHistory>> GetProductVariantApprovalHistory(int variantId);

}