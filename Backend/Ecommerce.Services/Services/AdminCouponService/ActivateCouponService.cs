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

    public async Task<ResponseAddCouponDTO> DeactivateCoupon(int couponId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateShipmentAndCouponAdminUserByUserId(adminUserId);

            _logger.LogInformation("Fetching coupon with id {CouponId}", couponId);

            var coupon = await _couponRepsository.Get(couponId);

            if (coupon == null)
            {
                _logger.LogWarning("Coupon with id {CouponId} not found", couponId);
                throw new DataNotFoundException($"Coupon with id {couponId} not found");
            }

            if (coupon.IsActive == false)
            {
                _logger.LogWarning("Coupon with id {CouponId} is already deactivated", couponId);
                throw new DataAlreadyRegisteredException("Coupon Already Deactivated");
            }

            bool previousIsActive = coupon.IsActive;
            coupon.IsActive = false;

            var updatedCoupon = await _couponRepsository.Update(coupon.CouponId, coupon);
            if (updatedCoupon == null)
            {
                _logger.LogError("Failed to deactivate Coupon with id {CouponId}", couponId);
                throw new DataRegistrationException("Coupon deactivation failed");
            }
            _logger.LogInformation("Coupon {CouponId} deactivated successfully", updatedCoupon.CouponId);

            var couponLog = new LogChanges
            {
                TableName = nameof(Coupons),
                RecordId = updatedCoupon.CouponId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"CouponId={couponId}, IsActive={previousIsActive}",
                NewValue = $"CouponId={updatedCoupon.CouponId}, IsActive={updatedCoupon.IsActive}",
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
            _logger.LogError(ex, "Transaction failed while deactivating coupon. CouponId: {CouponId}", couponId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back while deactivating coupon. CouponId: {CouponId}", couponId);
            throw;
        }
    }

    public async Task<ResponseAddCouponDTO> ActivateCoupon(int couponId, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateShipmentAndCouponAdminUserByUserId(adminUserId);

            _logger.LogInformation("Fetching coupon with id {CouponId}", couponId);

            var coupon = await _couponRepsository.Get(couponId);

            if (coupon == null)
            {
                _logger.LogWarning("Coupon with id {CouponId} not found", couponId);
                throw new DataNotFoundException($"Coupon with id {couponId} not found");
            }

            if (coupon.IsActive == true)
            {
                _logger.LogWarning("Coupon with id {CouponId} is already activated", couponId);
                throw new DataAlreadyRegisteredException("Coupon Already Activated");
            }

            bool previousIsActive = coupon.IsActive;
            coupon.IsActive = true;

            var updatedCoupon = await _couponRepsository.Update(coupon.CouponId, coupon);
            if (updatedCoupon == null)
            {
                _logger.LogError("Failed to activate Coupon with id {CouponId}", couponId);
                throw new DataRegistrationException("Coupon activation failed");
            }
            _logger.LogInformation("Coupon {CouponId} activated successfully", updatedCoupon.CouponId);

            var couponLog = new LogChanges
            {
                TableName = nameof(Coupons),
                RecordId = updatedCoupon.CouponId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"CouponId={couponId}, IsActive={previousIsActive}",
                NewValue = $"CouponId={updatedCoupon.CouponId}, IsActive={updatedCoupon.IsActive}",
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
            _logger.LogError(ex, "Transaction failed while activating coupon. CouponId: {CouponId}", couponId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back while activating coupon. CouponId: {CouponId}", couponId);
            throw;
        }
    }
}