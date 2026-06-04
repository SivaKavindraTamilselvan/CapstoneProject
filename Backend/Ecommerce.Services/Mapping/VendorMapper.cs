using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class VendorMappingProfile : Profile
    {
        public VendorMappingProfile()
        {
            CreateMap<RequestAddInventoryDTO,Inventory>();
            CreateMap<Inventory,ResponseAddInventoryDTO>();

            CreateMap<RequestUpdateInventoryDTO,Inventory>();
            CreateMap<Inventory,ResponseUpdateInventoryDTO>();

            CreateMap<RequestAddCouponDTO,Coupons>();
            CreateMap<Coupons,ResponseAddCouponDTO>();

            CreateMap<RequestAddCouponProductDTO,CouponsProduct>();
            CreateMap<Coupons,ResponseAddCouponProductDTO>();
            
        }
    }
}