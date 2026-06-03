using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO,int adminUserId)
    {
        var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
        var adminUser = (await _adminUserRepsository.GetAll()).FirstOrDefault(u=>u.UserId == adminUserId);
        if(adminUser == null)
        {
            throw new DataNotFoundException("Admin Not Found");
        }
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewedByAdminId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        await _productRepsository.Update(product.ProductId,product);
        await _approvalHistoryRepsository.Create(approvalHistory);
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
}
