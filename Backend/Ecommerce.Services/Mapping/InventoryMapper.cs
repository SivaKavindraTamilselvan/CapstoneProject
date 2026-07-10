using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            CreateMap<Inventory,ResponseAdminInventoryDTO>()
            .ForMember(dest=>dest.VendorName,opt=>opt.MapFrom(a=>a.ProductVariant!.Product!.Vendor!.VendorCompanyName))
            .ForMember(dest=>dest.VendorId,opt=>opt.MapFrom(a=>a.ProductVariant!.Product!.VendorId))
            .ForMember(dest=>dest.ContactPhoneNumber,opt=>opt.MapFrom(a=>a.Address!.ContactPhoneNumber))
            .ForMember(dest=>dest.PinCode,opt=>opt.MapFrom(a=>a.Address!.PinCode))
            .ForMember(dest=>dest.Country,opt=>opt.MapFrom(a=>a.Address!.Country))
            .ForMember(dest=>dest.City,opt=>opt.MapFrom(a=>a.Address!.City))
            .ForMember(dest=>dest.State,opt=>opt.MapFrom(a=>a.Address!.State))
            .ForMember(dest=>dest.AddressLine,opt=>opt.MapFrom(a=>a.Address!.AddressLine))
            .ForMember(dest=>dest.SKU,opt=>opt.MapFrom(a=>a.ProductVariant!.SKU));

            CreateMap<Inventory,ResponseVendorInventoryDTO>()
            .ForMember(dest=>dest.ContactPhoneNumber,opt=>opt.MapFrom(a=>a.Address!.ContactPhoneNumber))
            .ForMember(dest=>dest.PinCode,opt=>opt.MapFrom(a=>a.Address!.PinCode))
            .ForMember(dest=>dest.Country,opt=>opt.MapFrom(a=>a.Address!.Country))
            .ForMember(dest=>dest.City,opt=>opt.MapFrom(a=>a.Address!.City))
            .ForMember(dest=>dest.State,opt=>opt.MapFrom(a=>a.Address!.State))
            .ForMember(dest=>dest.AddressLine,opt=>opt.MapFrom(a=>a.Address!.AddressLine))
            .ForMember(dest=>dest.SKU,opt=>opt.MapFrom(a=>a.ProductVariant!.SKU));
        }
    }
}