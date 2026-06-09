using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<RequestCreateOrderDTO, Order>();
            CreateMap<Payment, ResponsePayment>();

            CreateMap<RequestAddReturnDTO, Return>();
            CreateMap<Return, ResponseAddReturnDTO>();

            CreateMap<Return, ResponseReviewReturnDTO>();
            CreateMap<OrderItems, OrderItemSummaryDto>()
            .ForMember(dest => dest.SKU,
                opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.SKU : ""))
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null
                    ? src.ProductVariant.Product.ProductName : ""))
            .ForMember(dest => dest.VendorName,
                opt => opt.MapFrom(src => src.ProductVariant != null
                    && src.ProductVariant.Product != null
                    && src.ProductVariant.Product.Vendor != null
                    ? src.ProductVariant.Product.Vendor.VendorCompanyName : ""))
            .ForMember(dest => dest.OrderItemStatus,
                opt => opt.MapFrom(src => src.OrderItemStatus != null ? src.OrderItemStatus.OrderItemStatusName : ""));

            // Order → OrderSummaryDto
            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Users != null ? src.Users.FirstName + " " + src.Users.LastName : ""))
                .ForMember(dest => dest.OrderStatus,
                    opt => opt.MapFrom(src => src.OrderStatus != null ? src.OrderStatus.OrderStatusName : ""))
                .ForMember(dest => dest.TotalItems,
                    opt => opt.MapFrom(src => src.OrderItems.Count))
                .ForMember(dest => dest.OrderItems,
                    opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}