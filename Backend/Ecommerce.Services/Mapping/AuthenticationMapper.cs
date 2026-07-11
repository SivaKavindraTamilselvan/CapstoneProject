using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class AuthenticationMappingProfile : Profile
    {
        public AuthenticationMappingProfile()
        {
            // user registration
            CreateMap<RequestRegisterUserDTO, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.HashedKey, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.Trim()))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.Trim()));
            CreateMap<User, ResponseRegisterUserDTO>();

            // vendor registration
            CreateMap<RequestRegisterVendorDTO, Vendor>()
            .ForMember(dest => dest.ContactPersonName, opt => opt.MapFrom(src => src.ContactPersonName.Trim()))
            .ForMember(dest => dest.CompanyEmail, opt => opt.MapFrom(src => src.CompanyEmail.Trim()))
            .ForMember(dest => dest.GSTNumber, opt => opt.MapFrom(src => src.GSTNumber.Trim()))
            .ForMember(dest => dest.VendorCompanyName, opt => opt.MapFrom(src => src.VendorCompanyName.Trim()));
            CreateMap<Vendor, ResponseRegisterVendorDTO>();
            // vendor user registration (done by the vendor owner)
            CreateMap<VendorUser, ResponseRegisterVendorUserDTO>();
            // admin user registration (done by main admin)
            CreateMap<AdminUser, ResponseRegisterAdminDTO>();

            // user login
            CreateMap<User, ResponseLoginUserDTO>()
            .ForMember(dest => dest.Token, opt => opt.Ignore());
            CreateMap<User, TokenRequest>()
            .ForMember(dest => dest.AdminRoleId, opt => opt.MapFrom(src => src.AdminUsers != null ? src.AdminUsers.AdminRoleId : (int?)null))
            .ForMember(dest => dest.VendorRoleId, opt => opt.MapFrom(src => src.VendorUser != null ? src.VendorUser.VendorRoleId : (int?)null));
        }
    }
}