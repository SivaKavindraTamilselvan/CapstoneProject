using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO,int adminUserId)
    {
        var product = await _productValidation.ValidateProduct(requestDeleteProductDTO.ProductId);
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewedByAdminId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestDeleteProductDTO.Remarks;
        approvalHistory.NewStatusId = 6;
        product.ProductApprovalStatusId = 6;
        product.UpdatedAt = DateTime.Now;
        await _productRepsository.Update(product.ProductId,product);
        await _approvalHistoryRepsository.Create(approvalHistory);
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
}