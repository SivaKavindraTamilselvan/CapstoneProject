using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class ReturnMappingProfile : Profile
    {
        public ReturnMappingProfile()
        {
            // return added by the user (request)
            CreateMap<RequestAddReturnDTO, Return>();
            CreateMap<Return, ResponseAddReturnDTO>();

            // review the return (admin and the vendor)
            CreateMap<Return, ResponseReviewReturnDTO>();

            // return summary for the admin and vendor
            CreateMap<Return, ReturnSummaryDto>()
            .ForMember(dest => dest.OrderItemId,opt => opt.MapFrom(src => src.OrderItemId))
            .ForMember(dest => dest.ProductName,opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.ProductName))
            .ForMember(dest => dest.SKU,opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.SKU))
            .ForMember(dest => dest.VendorName,opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.Vendor!.VendorCompanyName))
            .ForMember(dest => dest.OrderedQuantity,opt => opt.MapFrom(src => src.OrderItems!.Quantity))
            .ForMember(dest => dest.UnitPrice,opt => opt.MapFrom(src => src.OrderItems!.UnitPrice))
            .ForMember(dest => dest.Discount,opt => opt.MapFrom(src => src.OrderItems!.Discount))
            .ForMember(dest => dest.ReturnAmount,opt => opt.MapFrom(src =>(src.ReturnQuantity * src.OrderItems!.UnitPrice) - src.OrderItems.Discount))
            .ForMember(dest => dest.ReturnReason,opt => opt.MapFrom(src => src.ReturnReason!.ReturnReasonDescription))
            .ForMember(dest => dest.ReturnStatus,opt => opt.MapFrom(src => src.ReturnStatus!.ReturnStatusName))
            .ForMember(dest => dest.InventoryId,opt => opt.MapFrom(src => src.OrderItems!.InventoryId))
            .ForMember(dest => dest.InventoryCity,opt => opt.MapFrom(src => src.OrderItems!.Inventory!.Address!.City))
            .ForMember(dest => dest.InventoryAddress,opt => opt.MapFrom(src => src.OrderItems!.Inventory!.Address!.AddressLine))
            .ForMember(dest => dest.DeliveryCity,opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.City))
            .ForMember(dest => dest.DeliveryAddress,opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.AddressLine))
            .ForMember(dest => dest.DeliveryPincode,opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.PinCode));

        }
    }
}