using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class ShipmentMappingProfile : Profile
    {
        public ShipmentMappingProfile()
        {
            CreateMap<RequestAddShipmentDTO, Shipment>();
            CreateMap<Shipment, ResponseAddShipmentDTO>();

            CreateMap<RequestAddShipmentTrackingDTO, ShipmentTracking>();
            CreateMap<ShipmentTracking, ResponseAddShipmentTrackingDTO>();

            CreateMap<RequestAddShipmentDTO, ShipmentTracking>();

            CreateMap<Shipment, ShipmentDetailResponseDto>()
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.ShipmentStatus!.ShipmentStatusName))
            .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => src.CourierName != null ? src.CourierName : "Unassigned"))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Order != null && src.Order.Users != null ? src.Order.Users.FirstName + " " + src.Order.Users.LastName : string.Empty))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Order != null && src.Order.Users != null ? src.Order.Users.Email : string.Empty))
            .ForMember(dest => dest.PickupAddress, opt => opt.MapFrom(src => src.PickupAddress != null ? $"{src.PickupAddress.AddressLine}, {src.PickupAddress.City}, {src.PickupAddress.State} - {src.PickupAddress.PinCode}" : string.Empty))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ShipmentItems))
            .ForMember(dest => dest.Tracking, opt => opt.MapFrom(src => src.ShipmentTrackings));

            CreateMap<ShipmentItems, ShipmentItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.ProductName))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.SKU))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.OrderItems!.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OrderItems!.UnitPrice));

            CreateMap<ShipmentTracking, ShipmentTrackingResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ShipmentStatus!.ShipmentStatusName));

            CreateMap<Shipment, ShipmentSummaryResponseDto>()
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.ShipmentStatus!.ShipmentStatusName))
            .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => src.CourierName != null ? src.CourierName : "Unassigned"))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Order!.Users!.FirstName + " " + src.Order.Users.LastName))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.ShipmentItems.Count));

            CreateMap<Shipment, ShipmentStatusResponseDTO>();
        }

    }
}