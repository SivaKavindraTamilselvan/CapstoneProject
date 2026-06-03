using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ResponseAddProduct>();
            CreateMap<ProductImage, RequestAddProductImage>();
            CreateMap<ProductVariant, ResponseAddProductVariantDTO>();
            CreateMap<ProductVariantAttribute, ResponseAddProductVariantAttributeDTO>();

            CreateMap<Product, ResponseReviewOfProductDTO>();

            CreateMap<ProductCategory, ResponseAddProductCategoryDTO>();
            CreateMap<ProductSubCategory, ResponseAddProductSubCategoryDTO>();
            CreateMap<AttributeMaster, ResponseAddAttributeDTO>();
            CreateMap<ProductSubCategoryAttribute, ResponseAddProductSubCategoryAttributeDTO>();
        }
    }
}