using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<RequestAddProduct, Product>();
            CreateMap<Product, ResponseAddProduct>();

            CreateMap<Product, ResponseReviewOfProductDTO>();

            CreateMap<RequestUpdateProduct, Product>();
            CreateMap<Product, ResponseUpdateProduct>();

            CreateMap<Product, ResponseUserGetProductDetailDTO>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName));

            CreateMap<Product, ResponseVendorGetAllProductDTO>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.MainProductSubCategoryAttributeName, opt => opt.MapFrom(src => src.MainProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName));

            CreateMap<Product, ResponseAdminGetAllProductDTO>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.MainProductSubCategoryAttributeName, opt => opt.MapFrom(src => src.MainProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName))
            .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages.Where(img => img.ProductVariantId == null)));

            CreateMap<RequestAddProductVariantDTO, ProductVariant>();
            CreateMap<ProductVariant, ResponseAddProductVariantDTO>();

            CreateMap<ProductVariant, ResponseReviewOfProductVariantDTO>();

            CreateMap<RequestUpdateProductVariant, ProductVariant>();
            CreateMap<ProductVariant, ResponseUpdateProductVariantDTO>();

            CreateMap<ProductVariant, ResponseUserProductVariant>()
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.MinimumQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<ProductVariant, ResponseVendorGetProductVariant>()
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductVariantStatus, opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
            .ForMember(dest => dest.MinimuQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<ProductVariant, ResponseAdminProductVariantDTO>()
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductVariantStatus, opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
            .ForMember(dest => dest.MinimumQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<RequestAddProductVariantAttributeDTO, ProductVariantAttribute>();
            CreateMap<ProductVariantAttribute, ResponseAddProductVariantAttributeDTO>();

            CreateMap<ProductVariantAttribute, ResponseProductVariantAttributeDTO>()
            .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.ProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.AttributeValue, opt => opt.MapFrom(src => src.AttributeValue));

            CreateMap<ProductVariantAttribute, ResponseProductVariantAttribute>()
            .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.ProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.AttributeValue, opt => opt.MapFrom(src => src.AttributeValue));
        }
    }
}