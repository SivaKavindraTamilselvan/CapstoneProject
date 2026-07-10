using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class VendorMappingProfile : Profile
    {
        public VendorMappingProfile()
        {
            // get vendor for the admin dashboard
            CreateMap<Vendor, ResponseAdminGetVendorDTO>()
            .ForMember(dest => dest.ApprovalStatusName, opt => opt.MapFrom(src => src.ApprovalStatus!.ApprovalStatusName))
            .ForMember(dest=>dest.ReviewAdminName,opt=>opt.MapFrom(src=>src.ReviwedByAdmin!.User!.FirstName  + " " + src.ReviwedByAdmin.User.LastName));
            // add inventory by the vendor
            CreateMap<RequestAddInventoryDTO, Inventory>();
            CreateMap<Inventory, ResponseAddInventoryDTO>();
            // update vendor inventory (Active and quantity)
            CreateMap<RequestUpdateInventoryDTO, Inventory>();
            CreateMap<Inventory, ResponseUpdateInventoryDTO>();
            CreateMap<Inventory, ResponseAdminInventoryDTO>();
            CreateMap<Inventory, ResponseVendorInventoryDTO>();

            CreateMap<Vendor, ResponseGetVendor>()
            .ForMember(dest => dest.ApprovalStatusName, opt => opt.MapFrom(src => src.ApprovalStatus!.ApprovalStatusName));

            CreateMap<Vendor, ResponseReviewOfVendorDTO>();
        }
    }
}