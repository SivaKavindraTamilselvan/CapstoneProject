using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<ResponseReviewOfVendorDTO> DeleteVendor(DeleteVendorDto deleteVendorDto, int adminUserId)
    {
        var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
            var vendor = await _vendorRepsository.Get(deleteVendorDto.VendorId);
            if (vendor == null)
            {
                _logger.LogWarning("Vendor review failed. VendorId {VendorId} not found", deleteVendorDto.VendorId);
                throw new DataNotFoundException("Vendor ID Not Found");
            }
            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.EntityId = vendor.VendorId;
            approvalHistory.PreviousStatusId = vendor.ApprovalStatusId;
            approvalHistory.NewStatusId = (int)ApprovalStatusEnum.Deleted_By_Admin;
            approvalHistory.Remarks = deleteVendorDto.Remark;
            approvalHistory.ReviewedByAdminId = adminUser.AdminUserId;
            approvalHistory.ReviewedAt = DateTime.Now;
            await _approvalHistoryRepsository.Create(approvalHistory);

            vendor.ApprovalStatusId = (int)ApprovalStatusEnum.Deleted_By_Admin;
            vendor.ReviewedAt = DateTime.Now;
            vendor.ReviewedByAdminId = adminUser.AdminUserId;

            var ownerVendorUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
            if (ownerVendorUser != null)
            {
                await _notificationService.SendToUser(
                    ownerVendorUser.UserId,
                    "Vendor Deleted",
                    deleteVendorDto.Remark,
                    1,
                    "Vendor",
                    vendor.VendorId
                );
            }
            await _vendorRepsository.Update(deleteVendorDto.VendorId, vendor);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseReviewOfVendorDTO>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing transaction");
            await transaction.RollbackAsync();
            throw;
        }
    }
}