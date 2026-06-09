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

            CreateMap<RequestAddProductImage, ProductImage>();
            CreateMap<ProductImage, RequestAddProductImage>();

            CreateMap<RequestAddProductVariantDTO, ProductVariant>();
            CreateMap<ProductVariant, ResponseAddProductVariantDTO>();

            CreateMap<ProductImage, ResponseAddProductImage>();

            CreateMap<RequestAddProductVariantAttributeDTO, ProductVariantAttribute>();
            CreateMap<ProductVariantAttribute, ResponseAddProductVariantAttributeDTO>();

            CreateMap<Product, ResponseReviewOfProductDTO>();

            CreateMap<ProductCategory, ResponseAddProductCategoryDTO>();
            CreateMap<ProductSubCategory, ResponseAddProductSubCategoryDTO>();
            CreateMap<AttributeMaster, ResponseAddAttributeDTO>();
            CreateMap<ProductSubCategoryAttribute, ResponseAddProductSubCategoryAttributeDTO>();

            CreateMap<RequestUpdateProduct, Product>();
            CreateMap<Product, ResponseUpdateProduct>();

            CreateMap<ProductVariant, ResponseUpdateProductVariantDTO>();

            CreateMap<ProductImage, ResponseMakeDefaultImageDTO>();
            CreateMap<ProductImage, ResponseAddProductVariantImage>();

            CreateMap<Product, RequestUpdateProductStatus>();


            CreateMap<ProductCategory, ResponseUserGetAllCategory>();
            CreateMap<ProductCategory, ResponseAdminGetAllCategory>()
    .ForMember(dest => dest.AddedUserName,
        opt => opt.MapFrom(src =>
            src.AddedByAdminUser != null &&
            src.AddedByAdminUser.User != null
                ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName
                : null));

            CreateMap<ProductSubCategory, ResponseUserGetAllSubCategory>();
            CreateMap<ProductSubCategory, ResponseAdminGetAllSubCategory>()
    .ForMember(dest => dest.ProductSubCategoryName,
        opt => opt.MapFrom(src => src.ProductSubCategoryName))
    .ForMember(dest => dest.CategoryIsActive,
        opt => opt.MapFrom(src => src.ProductCategory!.IsActive))
    .ForMember(dest => dest.AddedUserName,
        opt => opt.MapFrom(src =>
            src.AddedByAdminUser != null &&
            src.AddedByAdminUser.User != null
                ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName
                : null));
            CreateMap<ProductSubCategory, ResponseVendorGetAllProductSubCategory>();

            CreateMap<AttributeMaster, ResponseAdminGetAttribute>()
    .ForMember(dest => dest.AddedUserName,
        opt => opt.MapFrom(src =>
            src.AddedByAdminUser != null &&
            src.AddedByAdminUser.User != null
                ? src.AddedByAdminUser.User.FirstName + " " + src.AddedByAdminUser.User.LastName
                : null));

            CreateMap<ProductSubCategoryAttribute, ResponseAdminGetCategoryAttribute>()
                .ForMember(dest => dest.ProductSubCategoryName,
                    opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.IsSubCategoryActive,
                    opt => opt.MapFrom(src => src.ProductSubCategory!.IsActive))
                .ForMember(dest => dest.AttributeName,
                    opt => opt.MapFrom(src => src.AttributeMaster!.AttributeName))
                .ForMember(dest => dest.IsAttributeActive,
                    opt => opt.MapFrom(src => src.AttributeMaster!.IsActive));

            CreateMap<ProductSubCategoryAttribute, ResponseGetAllProductSubCategoryAttribute>()
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src!.AttributeMaster!.AttributeName));

            // Shared
            CreateMap<ProductImage, ResponseProductImageDTO>()
                .ForMember(dest => dest.DisplayOrderName,
                           opt => opt.MapFrom(src => src.DisplayOrder!.DisplayOrderName));

            CreateMap<ProductVariantAttribute, ResponseProductVariantAttributeDTO>()
                .ForMember(dest => dest.AttributeName,
                           opt => opt.MapFrom(src => src.ProductSubCategoryAttribute!.AttributeMaster!.AttributeName));

            CreateMap<Inventory, ResponseInventoryDTO>();

            // User
            CreateMap<Product, ResponseUserGetAllProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.VendorName,
                           opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName));

            CreateMap<ProductVariant, ResponseUserProductVariantDTO>()
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
                .ForMember(dest => dest.MinimumQuantityPerUser,
                           opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
                .ForMember(dest => dest.Attributes,
                           opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<Product, ResponseUserGetProductDetailDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.ProductCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName))
                .ForMember(dest => dest.VendorName,
                           opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName));

            // Vendor
            CreateMap<Product, ResponseVendorGetAllProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.ProductApprovalStatus,
                           opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
                .ForMember(dest => dest.ProductStatus,
                           opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName));

            CreateMap<ProductVariant, ResponseVendorProductVariantDTO>()
                .ForMember(dest => dest.ProductApprovalStatus,
                           opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
                .ForMember(dest => dest.ProductVariantStatus,
                           opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
                .ForMember(dest => dest.ReservedQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
                .ForMember(dest => dest.MinimumQuantityPerUser,
                           opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
                .ForMember(dest => dest.Attributes,
                           opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<Product, ResponseVendorGetDraftProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.ProductStatus,
                           opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName));

            CreateMap<Product, ResponseVendorGetStockProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName));

            CreateMap<ProductVariant, ResponseVendorStockVariantDTO>()
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
                .ForMember(dest => dest.ReservedQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
                .ForMember(dest => dest.Attributes,
                           opt => opt.MapFrom(src => src.ProductVariantAttributes));

            // Admin
            CreateMap<Product, ResponseAdminGetAllProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.ProductCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName))
                .ForMember(dest => dest.VendorName,
                           opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName))
                .ForMember(dest => dest.ProductApprovalStatus,
                           opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
                .ForMember(dest => dest.ProductStatus,
                           opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName));

            CreateMap<ProductVariant, ResponseAdminProductVariantDTO>()
                .ForMember(dest => dest.ProductApprovalStatus,
                           opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
                .ForMember(dest => dest.ProductVariantStatus,
                           opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
                .ForMember(dest => dest.ReservedQuantity,
                           opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
                .ForMember(dest => dest.MinimumQuantityPerUser,
                           opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
                .ForMember(dest => dest.Attributes,
                           opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<Product, ResponseAdminGetPendingProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.VendorName,
                           opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName));

            CreateMap<Product, ResponseAdminGetStockProductDTO>()
                .ForMember(dest => dest.ProductSubCategoryName,
                           opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
                .ForMember(dest => dest.VendorName,
                           opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName));
        }
    }
}