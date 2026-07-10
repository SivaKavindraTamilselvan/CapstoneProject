using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class ImageMappingProfile : Profile
    {
        public ImageMappingProfile()
        {
            // add product image
            CreateMap<RequestAddProductImage, ProductImage>();
            CreateMap<ProductImage, ResponseAddProductImage>();

            // product variant image
            CreateMap<RequestAddProductVariantImage,ProductImage>();
            CreateMap<ProductImage,ResponseAddProductVariantImage>();

            // response for default image
            CreateMap<ProductImage, ResponseMakeDefaultImageDTO>();

            // for displaying image
            CreateMap<ProductImage, ResponseProductImageDTO>()
            .ForMember(dest => dest.DisplayOrderName,opt => opt.MapFrom(src => src.DisplayOrder!.DisplayOrderName));

        }
    }
}