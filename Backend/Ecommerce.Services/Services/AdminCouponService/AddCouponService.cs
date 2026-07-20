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

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO, int roleId, int UserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateShipmentAndCouponAdminUserByUserId(UserId);

            _logger.LogInformation("UserId {UserId} adding coupon with CouponCode {CouponCode}", UserId, requestAddCouponDTO.CouponCode);

            await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);

            var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
            if (coupon == null)
            {
                _logger.LogError("Failed to map RequestAddCouponDTO to Coupons entity. CouponCode: {CouponCode}", requestAddCouponDTO.CouponCode);
                throw new NullReferenceException("Coupon mapping failed");
            }

            if (roleId == (int)RoleEnum.Admin)
            {
                coupon.CouponTypeId = (int)CouponTypeEnum.Admin;
            }
            else
            {
                coupon.CouponTypeId = (int)CouponTypeEnum.Vendor;
            }
            coupon.CreatedByUserId = UserId;

            var createdCoupon = await _couponRepsository.Create(coupon);
            if (createdCoupon == null)
            {
                _logger.LogError("Failed to create Coupon with CouponCode {CouponCode}", requestAddCouponDTO.CouponCode);
                throw new DataRegistrationException("Coupon creation failed");
            }
            _logger.LogInformation("Coupon {CouponId} created successfully with CouponCode {CouponCode}", createdCoupon.CouponId, createdCoupon.CouponCode);

            var couponLog = new LogChanges
            {
                TableName = nameof(Coupons),
                RecordId = createdCoupon.CouponId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"CouponId={createdCoupon.CouponId}, CouponCode={createdCoupon.CouponCode}, CouponTypeId={createdCoupon.CouponTypeId}, DiscountValue={createdCoupon.DiscountValue}",
                UserId = UserId,
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
            return _mapper.Map<ResponseAddCouponDTO>(createdCoupon);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while adding coupon {Code}", requestAddCouponDTO.CouponCode);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for adding coupon {Code}", requestAddCouponDTO.CouponCode);
            throw;
        }
    }
}