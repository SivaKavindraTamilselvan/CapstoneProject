using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class CouponService : ICouponService
{


    public async Task<ResponseAddCouponDTO> UpdateCouponByIdForAdmin(UpdateCouponDto update, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateShipmentAndCouponAdminUserByUserId(adminUserId);

            _logger.LogInformation("Fetching coupon with id {CouponId}", update.CouponId);

            var coupon = await _couponRepsository.Get(update.CouponId);

            if (coupon == null)
            {
                _logger.LogWarning("Coupon with id {CouponId} not found", update.CouponId);
                throw new DataNotFoundException($"Coupon with id {update.CouponId} not found");
            }

            var oldValue = $"CouponId={coupon.CouponId}, CouponDescription={coupon.CouponDescription}, DiscountValue={coupon.DiscountValue}, " +
                            $"MinimumOrderAmount={coupon.MinimumOrderAmount}, StartDate={coupon.StartDate}, EndDate={coupon.EndDate}, " +
                            $"MinimumNumberOfUsage={coupon.MinimumNumberOfUsage}, IsActive={coupon.IsActive}";

            if (update.CouponDescription != null)
            {
                coupon.CouponDescription = update.CouponDescription;
            }

            if (update.DiscountValue.HasValue)
            {
                coupon.DiscountValue = update.DiscountValue.Value;
            }

            if (update.MinimumOrderAmount.HasValue)
            {
                coupon.MinimumOrderAmount = update.MinimumOrderAmount.Value;
            }

            if (update.StartDate.HasValue)
            {
                coupon.StartDate = update.StartDate.Value;
            }

            if (update.EndDate.HasValue)
            {
                coupon.EndDate = update.EndDate.Value;
            }

            if (update.MinimumNumberOfUsage.HasValue)
            {
                coupon.MinimumNumberOfUsage = update.MinimumNumberOfUsage.Value;
            }

            if (update.IsActive.HasValue)
            {
                coupon.IsActive = update.IsActive.Value;
            }

            var updatedCoupon = await _couponRepsository.Update(coupon.CouponId, coupon);
            if (updatedCoupon == null)
            {
                _logger.LogError("Failed to update Coupon with id {CouponId}", coupon.CouponId);
                throw new DataRegistrationException("Coupon update failed");
            }
            _logger.LogInformation("Coupon {CouponId} updated successfully", updatedCoupon.CouponId);

            var couponLog = new LogChanges
            {
                TableName = nameof(Coupons),
                RecordId = updatedCoupon.CouponId,
                Actions = (int)AuditAction.Updated,
                OldValue = oldValue,
                NewValue = $"CouponId={updatedCoupon.CouponId}, CouponDescription={updatedCoupon.CouponDescription}, DiscountValue={updatedCoupon.DiscountValue}, " +
                           $"MinimumOrderAmount={updatedCoupon.MinimumOrderAmount}, StartDate={updatedCoupon.StartDate}, EndDate={updatedCoupon.EndDate}, " +
                           $"MinimumNumberOfUsage={updatedCoupon.MinimumNumberOfUsage}, IsActive={updatedCoupon.IsActive}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(couponLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", couponLog.TableName, couponLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", couponLog.TableName, couponLog.RecordId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddCouponDTO>(updatedCoupon);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while updating coupon. CouponId: {CouponId}", update.CouponId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back while updating coupon. CouponId: {CouponId}", update.CouponId);
            throw;
        }
    }
}