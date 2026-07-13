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
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.ContactPersonName, opt => opt.MapFrom(src => src.Address!= null ? src.Address.ContactName : ""))
            .ForMember(dest => dest.ContactPersonPhoneNumber, opt => opt.MapFrom(src => src.Address!= null ? src.Address.ContactPhoneNumber : ""))
            .ForMember(dest => dest.UserAddress, opt => opt.MapFrom(src => src.Address!= null ? src.Address.AddressLine : ""))
            .ForMember(dest => dest.UserLandMark, opt => opt.MapFrom(src => src.Address!= null ? src.Address.LandMark : ""))
            .ForMember(dest => dest.UserState, opt => opt.MapFrom(src => src.Address!= null ? src.Address.State : ""))
            .ForMember(dest => dest.UserPincode, opt => opt.MapFrom(src => src.Address!= null ? src.Address.PinCode : ""))
            .ForMember(dest => dest.UserCity, opt => opt.MapFrom(src => src.Address!= null ? src.Address.City : ""));
            


            // for normal usage or API
            CreateMap<OrderItems, ResponseGetOrderItems>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant!.SKU))
            .ForMember(dest => dest.OrderItemStatusName, opt => opt.MapFrom(src => src.OrderItemStatus!.OrderItemStatusName));

            // order items response
            CreateMap<OrderItems, OrderItemSummaryDto>()


    .ForMember(dest => dest.SKU,
        opt => opt.MapFrom(src =>
            src.ProductVariant != null ? src.ProductVariant.SKU : ""))

     .ForMember(dest => dest.UserName,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.ContactName : ""))

 .ForMember(dest => dest.ContactPersonName,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.ContactName : ""))

             .ForMember(dest => dest.ContactPersonPhoneNumber,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.ContactPhoneNumber : ""))

.ForMember(dest => dest.UserAddress,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.AddressLine : ""))

.ForMember(dest => dest.UserLandMark,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.LandMark : ""))

.ForMember(dest => dest.UserCity,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.City : ""))

.ForMember(dest => dest.UserPincode,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.PinCode : ""))

.ForMember(dest => dest.UserPincode,
        opt => opt.MapFrom(src =>
            src.Order.AddressId != null ? src.Order.Address.PinCode : ""))



   .ForMember(dest => dest.ProductImageUrl,
    opt => opt.MapFrom(src =>
        src.ProductVariant != null && src.ProductVariant.Product != null
            ? (src.ProductVariant.Product.ProductImages
                .FirstOrDefault(pi => pi.IsMainImage) != null
                    ? src.ProductVariant.Product.ProductImages.First(pi => pi.IsMainImage).ImageUrl
                    : "")
            : ""))

    .ForMember(dest => dest.ProductName,
        opt => opt.MapFrom(src =>
            src.ProductVariant != null && src.ProductVariant.Product != null
                ? src.ProductVariant.Product.ProductName
                : ""))
     .ForMember(dest => dest.ProductId,
        opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.ProductId : 0))
 .ForMember(dest => dest.ProductVariantId,
        opt => opt.MapFrom(src => src.ProductVariantId))

    .ForMember(dest => dest.VendorName,
        opt => opt.MapFrom(src =>
            src.ProductVariant != null &&
            src.ProductVariant.Product != null &&
            src.ProductVariant.Product.Vendor != null
                ? src.ProductVariant.Product.Vendor.VendorCompanyName
                : ""))

    .ForMember(dest => dest.ItemTotal,
        opt => opt.MapFrom(src =>
            (src.UnitPrice * src.Quantity) - src.Discount))

    .ForMember(dest => dest.InventoryCity,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.City
                : ""))
     .ForMember(dest => dest.VendorContactPersonName,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.ContactName
                : ""))
     .ForMember(dest => dest.VendorContactPersonPhoneNumber,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.ContactPhoneNumber
                : ""))

    .ForMember(dest => dest.InventoryAddress,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.AddressLine
                : ""))
    .ForMember(dest => dest.InventoryLandMark,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.LandMark
                : ""))
    .ForMember(dest => dest.InventoryState,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.State
                : ""))
    .ForMember(dest => dest.InventoryPincode,
        opt => opt.MapFrom(src =>
            src.Inventory != null &&
            src.Inventory.Address != null
                ? src.Inventory.Address.PinCode
                : ""))

    .ForMember(dest => dest.OrderItemStatus,
        opt => opt.MapFrom(src =>
            src.OrderItemStatus != null
                ? src.OrderItemStatus.OrderItemStatusName
                : ""))
    .ForMember(dest => dest.returns,
    opt => opt.MapFrom(src => src.Returns))
     .ForMember(dest => dest.cancels,
    opt => opt.MapFrom(src => src.Cancels));

        }


    }
}