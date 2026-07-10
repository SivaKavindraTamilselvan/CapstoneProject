using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            // add the coupon
            CreateMap<RequestAddCouponDTO, Coupons>();
            CreateMap<Coupons, ResponseAddCouponDTO>();

            // get all the couponsxw
            CreateMap<Coupons, ResponseGetAllCoupon>();

            CreateMap<RequestAddCouponProductDTO, CouponsProduct>();
            CreateMap<Coupons, ResponseAddCouponProductDTO>();
        }
    }
}