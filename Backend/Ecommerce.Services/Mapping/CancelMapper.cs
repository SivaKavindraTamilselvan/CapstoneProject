using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class CancelMappingProfile : Profile
    {
        public CancelMappingProfile()
        {
            CreateMap<Cancel, CancelSummaryDto>()
           .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.OrderItemId))
           .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderItems!.OrderId))
           .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderItems!.Order!.OrderNumber))
           .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.ProductName))
           .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.SKU))
           .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.Vendor!.VendorCompanyName))
           .ForMember(dest => dest.OrderedQuantity, opt => opt.MapFrom(src => src.OrderItems!.Quantity))
           .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OrderItems!.UnitPrice))
           .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.OrderItems!.Discount))
           .ForMember(dest => dest.CancelAmount, opt => opt.MapFrom(src => (src.CancelQuantity * src.OrderItems!.UnitPrice) - src.ConvenienceFee))
           .ForMember(dest => dest.CancelReason, opt => opt.MapFrom(src => src.CancelReason!.CancelReasonDescription))
           .ForMember(dest => dest.CancelStatus, opt => opt.MapFrom(src => src.CancelStatus!.CancelStatusName))
           .ForMember(dest => dest.DeliveryCity, opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.City))
           .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.AddressLine))
           .ForMember(dest => dest.DeliveryPincode, opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.PinCode));
        }
    }
}