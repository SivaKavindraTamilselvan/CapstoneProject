using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<RequestAddProduct,Product>();
            CreateMap<Product, ResponseAddProduct>();

            CreateMap<RequestAddProductImage,ProductImage>();
            CreateMap<ProductImage, RequestAddProductImage>();

            CreateMap<RequestAddProductVariantDTO,ProductVariant>();
            CreateMap<ProductVariant, ResponseAddProductVariantDTO>();

            CreateMap<RequestAddProductVariantAttributeDTO, ProductVariantAttribute>();
            CreateMap<ProductVariantAttribute, ResponseAddProductVariantAttributeDTO>();

            CreateMap<Product, ResponseReviewOfProductDTO>();

            CreateMap<ProductCategory, ResponseAddProductCategoryDTO>();
            CreateMap<ProductSubCategory, ResponseAddProductSubCategoryDTO>();
            CreateMap<AttributeMaster, ResponseAddAttributeDTO>();
            CreateMap<ProductSubCategoryAttribute, ResponseAddProductSubCategoryAttributeDTO>();

        }
    }
}