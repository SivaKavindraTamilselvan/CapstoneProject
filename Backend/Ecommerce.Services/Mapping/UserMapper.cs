using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RequestRegisterUserDTO, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.HashedKey, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()));
            CreateMap<User, ResponseRegisterUserDTO>();

            CreateMap<RequestRegisterVendorDTO, Vendor>();
            CreateMap<Vendor, ResponseRegisterVendorDTO>();

            CreateMap<VendorUser, ResponseRegisterVendorUserDTO>();

            CreateMap<User, ResponseLoginUserDTO>()
            .ForMember(dest => dest.Token, opt => opt.Ignore());

            CreateMap<User, TokenRequest>()
            .ForMember(dest => dest.AdminRoleId, opt => opt.MapFrom(src => src.AdminUsers != null ? src.AdminUsers.AdminRoleId : (int?)null))
            .ForMember(dest => dest.VendorRoleId, opt => opt.MapFrom(src => src.VendorUser != null ? src.VendorUser.VendorRoleId : (int?)null));

            CreateMap<AdminUser, ResponseRegisterAdminDTO>();
            CreateMap<FavoritesItems, ResponseFavoriteItemsDTO>();
            CreateMap<CartItems, ResponseCartItemsDTO>();
            CreateMap<CartItems, ResponseGetCartDTO>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.ProductVariant!.Product!.ProductName))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.ProductVariant!.Product!.Description))
                .ForMember(dest => dest.ProductVariantId,
                    opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.SKU,
                    opt => opt.MapFrom(src => src.ProductVariant!.SKU))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.ProductVariant!.Price));
            CreateMap<Vendor, ResponseReviewOfVendorDTO>();

            CreateMap<RequestAddAddressDTO, Address>();
            CreateMap<Address, ResponseAddAddressDTO>();
            CreateMap<Address, ResponseGetAddressDTO>();

            CreateMap<Address, ResponseMakeDefaultAddressDTO>();
            CreateMap<Coupons, ResponseGetAllCoupon>();

            CreateMap<Order, ResponseAddOrderDTO>();
            CreateMap<Favorites, ResponseGetFavoriteDTO>();

            CreateMap<RequestAddReviewDTO, Reviews>();
            CreateMap<Reviews, ResponseAddReviewDTO>();

            CreateMap<AdminUser, ResponseGetAdminUserDTO>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User!.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.User!.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User!.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User!.PhoneNumber))
            .ForMember(d => d.AdminRoleName, o => o.MapFrom(s => s.AdminRole!.AdminRoleName));
        }
    }
}