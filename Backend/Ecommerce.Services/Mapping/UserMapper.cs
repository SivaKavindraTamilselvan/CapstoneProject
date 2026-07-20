using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            //favorites and cart items are added during the registration itself
            CreateMap<FavoritesItems, ResponseFavoriteItemsDTO>();
            CreateMap<CartItems, ResponseCartItemsDTO>();

            CreateMap<CartItems, ResponseGetCartDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant!.Product!.ProductName))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductVariant!.ProductId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductVariant!.Product!.Description))
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant!.SKU))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariant!.Price));

            CreateMap<Favorites, ResponseGetFavoriteDTO>();

            CreateMap<FavoritesItems, ResponseGetFavoriteDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant!.Product!.ProductName))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductVariant!.ProductId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductVariant!.Product!.Description))
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant!.SKU))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariant!.Price));


            // review are added by the user
            CreateMap<RequestAddReviewDTO, Reviews>();
            CreateMap<Reviews, ResponseAddReviewDTO>();

            // getting all the admin user for the admin
            CreateMap<AdminUser, ResponseGetAdminUserDTO>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User!.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.User!.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User!.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User!.PhoneNumber))
            .ForMember(d => d.AdminRoleName, o => o.MapFrom(s => s.AdminRole!.AdminRoleName));

            CreateMap<VendorUser, ResponseGetVendorUserDTO>()
           .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User!.FirstName))
           .ForMember(d => d.LastName, o => o.MapFrom(s => s.User!.LastName))
           .ForMember(d => d.Email, o => o.MapFrom(s => s.User!.Email))
           .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User!.PhoneNumber))
           .ForMember(d => d.VendorRoleName, o => o.MapFrom(s => s.VendorRole!.VendorRoleName));

            CreateMap<VendorUser, ResponseGetVendorUserListDTO>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User!.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.User!.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User!.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User!.PhoneNumber))
            .ForMember(d => d.VendorRoleName, o => o.MapFrom(s => s.VendorRole!.VendorRoleName));

            CreateMap<Notification, ResponseNotification>()
            .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationType != null ? src.NotificationType.TypeName : string.Empty));

            CreateMap<User, ResponseGetProfileDTO>();
        }
    }
}