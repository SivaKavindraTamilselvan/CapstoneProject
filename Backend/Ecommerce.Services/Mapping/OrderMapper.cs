using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // create order 
            CreateMap<RequestCreateOrderDTO, Order>();
            CreateMap<Order, ResponseAddOrderDTO>();

            CreateMap<Payment, ResponsePayment>();


            CreateMap<AdminOrderFilterParams, OrderFilterParams>();

            // ordersummary dto for admin,vendir,admin displaying
            CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Users != null ? src.Users.FirstName + " " + src.Users.LastName : ""))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus != null ? src.OrderStatus.OrderStatusName : ""))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.OrderItems.Count))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            // for normal usage or API
            CreateMap<OrderItems, ResponseGetOrderItems>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant!.SKU))
            .ForMember(dest => dest.OrderItemStatusName, opt => opt.MapFrom(src => src.OrderItemStatus!.OrderItemStatusName));

            // order items response
            CreateMap<OrderItems, OrderItemSummaryDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.SKU : ""))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null ? src.ProductVariant.Product.ProductName : ""))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null && src.ProductVariant.Product.Vendor != null ? src.ProductVariant.Product.Vendor.VendorCompanyName : ""))
            .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => (src.UnitPrice * src.Quantity) - src.Discount))
            .ForMember(dest => dest.InventoryCity, opt => opt.MapFrom(src => src.Inventory != null && src.Inventory.Address != null ? src.Inventory.Address.City : ""))
            .ForMember(dest => dest.InventoryAddress, opt => opt.MapFrom(src => src.Inventory != null && src.Inventory.Address != null ? src.Inventory.Address.AddressLine : ""))
            .ForMember(dest => dest.OrderItemStatus, opt => opt.MapFrom(src => src.OrderItemStatus != null ? src.OrderItemStatus.OrderItemStatusName : ""));
        }

    }
}