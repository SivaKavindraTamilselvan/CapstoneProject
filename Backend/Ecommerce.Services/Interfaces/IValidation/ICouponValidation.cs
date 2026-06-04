using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface ICouponValidation
{
    public Task<Coupons?> ValidateCouponCode(string code);

}