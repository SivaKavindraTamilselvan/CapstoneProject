using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.DTOs.Returns;
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
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.ProductName))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.SKU))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.OrderItems!.ProductVariant!.Product!.Vendor!.VendorCompanyName))
            .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => (src.ReturnQuantity * src.OrderItems!.UnitPrice) - src.OrderItems.Discount))
            .ForMember(dest => dest.ReturnReason, opt => opt.MapFrom(src => src.ReturnReason!.ReturnReasonDescription))
            .ForMember(dest => dest.ReturnStatus, opt => opt.MapFrom(src => src.ReturnStatus!.ReturnStatusName))
            .ForMember(dest => dest.InventoryCity, opt => opt.MapFrom(src => src.OrderItems!.Inventory!.Address!.City))
            .ForMember(dest => dest.DeliveryCity, opt => opt.MapFrom(src => src.OrderItems!.Order!.Address!.City));

            CreateMap<Return, ReturnDetailsDto>();

            CreateMap<Return, ReturnInformationDto>()
            .ForMember(d => d.ReturnStatus, o => o.MapFrom(s => s.ReturnStatus!.ReturnStatusName))
            .ForMember(d => d.ReturnReason, o => o.MapFrom(s => s.ReturnReason!.ReturnReasonDescription))
            .ForMember(d => d.ReviewedByVendorName, o => o.MapFrom(s => s.ReviewedByVendor == null ? null : s.ReviewedByVendor.User!.FirstName + " " + s.ReviewedByVendor.User!.LastName));

            CreateMap<Return, CustomerInformationDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.OrderItems!.Order!.Users!.UserId))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.OrderItems!.Order!.Users!.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.OrderItems!.Order!.Users!.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.OrderItems!.Order!.Users!.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.OrderItems!.Order!.Users!.PhoneNumber));

            CreateMap<Return, OrderInformationDto>()
            .ForMember(d => d.OrderId, o => o.MapFrom(s => s.OrderItems!.Order!.OrderId))
            .ForMember(d => d.OrderNumber, o => o.MapFrom(s => s.OrderItems!.Order!.OrderNumber))
            .ForMember(d => d.OrderDate, o => o.MapFrom(s => s.OrderItems!.Order!.OrderDate))
            .ForMember(d => d.OrderStatus, o => o.MapFrom(s => s.OrderItems!.Order!.OrderStatus!.OrderStatusName))
            .ForMember(d => d.TotalProductAmount, o => o.MapFrom(s => s.OrderItems!.Order!.TotalProductAmount))
            .ForMember(d => d.TotalShippingAmount, o => o.MapFrom(s => s.OrderItems!.Order!.TotalShippingAmount))
            .ForMember(d => d.TotalCouponAmount, o => o.MapFrom(s => s.OrderItems!.Order!.TotalCouponAmount))
            .ForMember(d => d.FinalAmount, o => o.MapFrom(s => s.OrderItems!.Order!.FinalAmount))
            .ForMember(d => d.OrderedQuantity, o => o.MapFrom(s => s.OrderItems!.Quantity))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.OrderItems!.UnitPrice))
            .ForMember(d => d.Discount, o => o.MapFrom(s => s.OrderItems!.Discount));

            CreateMap<Return, AddressInformationDto>()
            .ForMember(d => d.AddressId, o => o.MapFrom(s => s.OrderItems!.Order!.Address!.AddressId))
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Return, ProductInformationDto>()
            .ForMember(d => d.OrderItemId, o => o.MapFrom(s => s.OrderItems!.OrderItemsId))
            .ForMember(d => d.InventoryId, o => o.MapFrom(s => s.OrderItems!.InventoryId))
            .ForMember(d => d.ProductVariantId, o => o.MapFrom(s => s.OrderItems!.ProductVariantId))
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.ProductId))
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.Product!.ProductName))
            .ForMember(d => d.SKU, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.SKU))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.ProductImages.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.IsReturnAllowed, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.IsReturn))
            .ForMember(d => d.IsExchangeAllowed, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.IsExchange))
            .ForMember(d => d.Attributes, o => o.MapFrom(s => s.OrderItems!.ProductVariant!.ProductVariantAttributes));

            CreateMap<ProductVariantAttribute, ProductAttributeDto>()
            .ForMember(d => d.AttributeName, o => o.MapFrom(s => s.ProductSubCategoryAttribute!.AttributeMaster!.AttributeName))
            .ForMember(d => d.AttributeValue, o => o.MapFrom(s => s.AttributeValue));

            CreateMap<Return, ReturnDetailsDto>()
            .ForMember(d => d.Return, o => o.MapFrom(s => s))
            .ForMember(d => d.Customer, o => o.MapFrom(s => s))
            .ForMember(d => d.Order, o => o.MapFrom(s => s))
            .ForMember(d => d.Address, o => o.MapFrom(s => s.OrderItems!.Order!.Address))
            .ForMember(d => d.Product, o => o.MapFrom(s => s));
        }
    }
}