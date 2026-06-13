using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class ProductCategoryMappingProfile : Profile
    {
        public ProductCategoryMappingProfile()
        {
            CreateMap<ProductCategory, ResponseAddProductCategoryDTO>();
            CreateMap<ProductSubCategory, ResponseAddProductSubCategoryDTO>();

            CreateMap<AttributeMaster, ResponseAddAttributeDTO>();
            CreateMap<ProductSubCategoryAttribute, ResponseAddProductSubCategoryAttributeDTO>();

            CreateMap<ProductCategory, ResponseUserGetAllCategory>();
            CreateMap<ProductCategory, ResponseAdminGetAllCategory>()
            .ForMember(dest => dest.AddedUserName, opt => opt.MapFrom(src => src.AddedByAdminUser != null &&
            src.AddedByAdminUser.User != null ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName : null));

            CreateMap<ProductSubCategory, ResponseUserGetAllSubCategory>();
            CreateMap<ProductSubCategory, ResponseAdminGetAllSubCategory>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategoryName))
            .ForMember(dest => dest.CategoryIsActive, opt => opt.MapFrom(src => src.ProductCategory!.IsActive))
            .ForMember(dest => dest.AddedUserName, opt => opt.MapFrom(src => src.AddedByAdminUser != null && src.AddedByAdminUser.User != null ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName : null));
            CreateMap<ProductSubCategory, ResponseVendorGetAllProductSubCategory>();


            CreateMap<AttributeMaster, ResponseAdminGetAttribute>()
           .ForMember(dest => dest.AddedUserName, opt => opt.MapFrom(src => src.AddedByAdminUser != null && src.AddedByAdminUser.User != null ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName : null));

            CreateMap<ProductSubCategoryAttribute, ResponseAdminGetCategoryAttribute>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.IsSubCategoryActive, opt => opt.MapFrom(src => src.ProductSubCategory!.IsActive))
            .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.IsAttributeActive, opt => opt.MapFrom(src => src.AttributeMaster!.IsActive));

            CreateMap<ProductSubCategoryAttribute, ResponseGetAllProductSubCategoryAttribute>()
           .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src!.AttributeMaster!.AttributeName));

        }
    }
}