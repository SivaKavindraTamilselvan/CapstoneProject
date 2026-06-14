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

            CreateMap<RequestUpdateProduct, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AddedByVendorUserId, opt => opt.Ignore());
            CreateMap<Product, ResponseUpdateProduct>();

            CreateMap<Product, ResponseUserGetProductDetailDTO>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor!.VendorCompanyName))
            .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages.Where(img => img.ProductVariantId == null)));

            CreateMap<Product, ResponseVendorGetAllProductDTO>()
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.MainProductSubCategoryAttributeName, opt => opt.MapFrom(src => src.MainProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus!.ProductStatusName))
            .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages.Where(img => img.ProductVariantId == null)))
            .ForMember(dest => dest.AddedByVendorUser, opt => opt.MapFrom(src => src.AddedByVendorUser!.User!.FirstName + " " + src.AddedByVendorUser.User.LastName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductSubCategory!.ProductCategory!.ProductCategoryName));

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

            CreateMap<RequestUpdateProductVariant, ProductVariant>()
            .ForMember(dest => dest.ProductVariantId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AddedByVendorUserId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductApprovalStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariantStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariantAttributes, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.Inventories, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
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
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes))
            .ForMember(dest => dest.AddedByVendorUser, opt => opt.MapFrom(src => src.AddedByVendorUser!.User!.FirstName + " " + src.AddedByVendorUser.User.LastName));

            CreateMap<ProductVariant, ResponseVendorGetProductVariantOnly>()
            .ForMember(dest => dest.ProductVariantApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductVariantStatus, opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
            .ForMember(dest => dest.MinimuQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes))
             .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.ProductName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product!.Description))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.AddedByVendorUser, opt => opt.MapFrom(src => src.AddedByVendorUser!.User!.FirstName + " " + src.AddedByVendorUser.User.LastName))
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.Product!.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.Product!.ProductSubCategory!.ProductCategory!.ProductCategoryName))
            .ForMember(dest => dest.MainProductSubCategoryAttributeName, opt => opt.MapFrom(src => src.Product!.MainProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName));

            CreateMap<ProductVariant, ResponseAdminProductVariantDTO>()
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductVariantStatus, opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
            .ForMember(dest => dest.MinimumQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes));

            CreateMap<ProductVariant, ResponseAdminProductVariantOnlyDTO>()
            .ForMember(dest => dest.ProductVariantApprovalStatus, opt => opt.MapFrom(src => src.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductVariantStatus, opt => opt.MapFrom(src => src.ProductVariantStatus!.ProductStatusName))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.AvailableQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.Inventories.Sum(i => i.ReservedQuantity)))
            .ForMember(dest => dest.MinimumQuantityPerUser, opt => opt.MapFrom(src => src.MinimuQuantityPerUser))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.ProductVariantAttributes))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.ProductName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product!.Description))
            .ForMember(dest => dest.ProductSubCategoryName, opt => opt.MapFrom(src => src.Product!.ProductSubCategory!.ProductSubCategoryName))
            .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.Product!.ProductSubCategory!.ProductCategory!.ProductCategoryName))
            .ForMember(dest => dest.MainProductSubCategoryAttributeName, opt => opt.MapFrom(src => src.Product!.MainProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(dest => dest.ProductApprovalStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName))
            .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.Product!.ProductApprovalStatus!.ProductApprovalStatusName));


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