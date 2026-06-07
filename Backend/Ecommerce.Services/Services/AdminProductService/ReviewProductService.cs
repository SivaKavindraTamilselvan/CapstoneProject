using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO,int adminUserId)
    {
        var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
        if(product.ProductApprovalStatusId != 2)
        {
            throw new DataApprovalStatusException("Product is not approved by vendor yet");
        }
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewedByAdminId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.UpdatedAt = DateTime.Now;
        var updated = await _productRepsository.Update(product.ProductId,product);
        Console.WriteLine(updated.ProductApprovalStatusId);
        await _approvalHistoryRepsository.Create(approvalHistory);
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
}
