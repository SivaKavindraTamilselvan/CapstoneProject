using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProductByVendor(RequestReviewOfProductDTO requestReviewOfProductDTO, int vendorUserId)
    {
        var User = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        await _productValidation.VendorValidateProduct(requestReviewOfProductDTO.ProductId,User.VendorId);
        _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ProductId {ProductId} with StatusId {StatusId}", vendorUserId, requestReviewOfProductDTO.ProductId, requestReviewOfProductDTO.ApprovalStatusId);
        if (requestReviewOfProductDTO.ApprovalStatusId != 2 && requestReviewOfProductDTO.ApprovalStatusId != 3)
        {
            _logger.LogWarning("Invalid approval status {StatusId} provided by Vendor UserId {VendorUserId}", requestReviewOfProductDTO.ApprovalStatusId, vendorUserId);
            throw new InvalidOperationException("Vendor can only approve or reject a product");
        }
        var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
        if (product.ProductApprovalStatusId == 2 || product.ProductApprovalStatusId == 3)
        {
            _logger.LogWarning(
            "ProductId {ProductId} already reviewed with StatusId {StatusId}",
            product.ProductId,
            product.ProductApprovalStatusId);
            throw new InvalidOperationException("Product already reviewed");
        }

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
        _logger.LogInformation("ProductId {ProductId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}", product.ProductId, vendorUserId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);
        return _mapper.Map<ResponseReviewOfProductDTO>(updatedproduct);
    }
    public async Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int vendorUserId)
    {
        await _productValidation.ValidateProductVariant(requestReviewOfProductDTO.ProductVariantId,vendorUserId);
        _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ProductVariantId {ProductVariantId} with StatusId {StatusId}", vendorUserId, requestReviewOfProductDTO.ProductVariantId, requestReviewOfProductDTO.ApprovalStatusId);
        if (requestReviewOfProductDTO.ApprovalStatusId != 2 && requestReviewOfProductDTO.ApprovalStatusId != 3)
        {
            throw new InvalidOperationException("Vendor can only approve or reject a product");
        }
        var product = await _productValidation.AdminValidateProductVariant(requestReviewOfProductDTO.ProductVariantId);
        if (product.ProductApprovalStatusId == 2 || product.ProductApprovalStatusId == 3)
        {
            _logger.LogWarning("ProductVariantId {ProductVariantId} already reviewed with StatusId {StatusId}", product.ProductVariantId, product.ProductApprovalStatusId);
            throw new InvalidOperationException("Product already reviewed");
        }

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        await _productValidation.ValidateProductVariant(product.ProductVariantId, vendorUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product_Variant";
        approvalHistory.EntityId = product.ProductVariantId;
        approvalHistory.ReviewedByAdminId = vendorUser.VendorUserId;
        approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.UpdatedAt = DateTime.Now;
        var updated = await _productVariantRepsository.Update(product.ProductVariantId, product);
        await _approvalHistoryRepsository.Create(approvalHistory);
        _logger.LogInformation("ProductVariantId {ProductVariantId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}", product.ProductVariantId, vendorUserId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);
        return _mapper.Map<ResponseReviewOfProductVariantDTO>(product);
    }
}