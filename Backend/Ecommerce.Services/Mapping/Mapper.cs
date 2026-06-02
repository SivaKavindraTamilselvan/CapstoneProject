using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestRegisterUserDTO, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.HashedKey, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()));

            CreateMap<User, ResponseRegisterUserDTO>();

            CreateMap<User, ResponseLoginUserDTO>()
            .ForMember(dest => dest.Token, opt => opt.Ignore());

            CreateMap<User, TokenRequest>()
            .ForMember(dest => dest.AdminRoleId, opt => opt.MapFrom(src => src.AdminUsers != null ? src.AdminUsers.AdminRoleId : (int?)null))
            .ForMember(dest => dest.VendorRoleId, opt => opt.MapFrom(src => src.VendorUser != null ? src.VendorUser.VendorRoleId : (int?)null));

            CreateMap<AdminUser, ResponseRegisterAdminDTO>();

            CreateMap<RequestRegisterVendorDTO, Vendor>();
            CreateMap<Vendor, ResponseRegisterVendorDTO>();

            CreateMap<VendorUser,ResponseRegisterVendorUserDTO>();

            CreateMap<Product, ResponseAddProduct>();
            CreateMap<Product,ResponseReviewOfProductDTO>();

            CreateMap<ProductCategory,ResponseAddProductCategoryDTO>();
            CreateMap<ProductSubCategory,ResponseAddProductSubCategoryDTO>();
            CreateMap<AttributeMaster,ResponseAddAttributeDTO>();
            CreateMap<ProductSubCategoryAttribute,ResponseAddProductSubCategoryAttributeDTO>();

            CreateMap<FavoritesItems,ResponseFavoriteItemsDTO>();
            CreateMap<CartItems,ResponseCartItemsDTO>();

            CreateMap<ProductVariant,ResponseAddProductVariantDTO>();
            CreateMap<ProductImage,RequestAddProductImage>();

            CreateMap<Vendor,ResponseReviewOfVendorDTO>();
        }
    }
}