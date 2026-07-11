using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<RequestAddCouponDTO, Coupons>();
            CreateMap<Coupons, ResponseAddCouponDTO>();

            CreateMap<Coupons, ResponseGetAllCoupon>();

            CreateMap<RequestAddCouponProductDTO, CouponsProduct>();
            CreateMap<Coupons, ResponseAddCouponProductDTO>();

            CreateMap<Coupons, CouponListDto>()
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.EndDate < DateTime.Now))
            .ForMember(dest => dest.CouponTypeName, opt => opt.MapFrom(src => src.CouponType != null ? src.CouponType.CouponTypeName : string.Empty))
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.CouponUsages.Count));

            CreateMap<Coupons, CouponDetailDto>()
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.EndDate < DateTime.Now))
            .ForMember(dest => dest.CouponTypeName, opt => opt.MapFrom(src => src.CouponType != null ? src.CouponType.CouponTypeName : string.Empty))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.FirstName + " " + src.CreatedByUser.LastName : string.Empty))
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.CouponUsages.Count))
            .ForMember(dest => dest.ApplicableProductIds, opt => opt.Ignore())
            .ForMember(dest => dest.UsageHistory, opt => opt.Ignore());
        }
    }
}