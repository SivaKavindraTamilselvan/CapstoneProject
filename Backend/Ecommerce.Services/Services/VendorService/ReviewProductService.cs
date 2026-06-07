using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorService : IVendorService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProductByVendor(RequestReviewOfProductDTO requestReviewOfProductDTO, int vendorUserId)
    {
        if (requestReviewOfProductDTO.ApprovalStatusId != 2 && requestReviewOfProductDTO.ApprovalStatusId != 3)
        {
            throw new InvalidOperationException("Vendor can only approve or reject a product");
        }

        var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewedByAdminId = vendorUser.VendorUserId;
        approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product = await _productRepsository.Update(product.ProductId, product);
        var updatedproduct = await _productRepsository.Get(product!.ProductId);
        await _approvalHistoryRepsository.Create(approvalHistory);
        return _mapper.Map<ResponseReviewOfProductDTO>(updatedproduct);
    }

}