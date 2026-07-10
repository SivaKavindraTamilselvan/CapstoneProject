using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            // add address
            CreateMap<RequestAddAddressDTO, Address>()
            .ForMember(dest=>dest.AddressLine,dest=>dest.MapFrom(src=>src.AddressLine.Trim()))
            .ForMember(dest=>dest.ContactName,dest=>dest.MapFrom(src=>src.ContactName.Trim()))
            .ForMember(dest=>dest.LandMark,dest=>dest.MapFrom(src=>src.LandMark.Trim()))
            .ForMember(dest=>dest.City,dest=>dest.MapFrom(src=>src.City.Trim()))
            .ForMember(dest=>dest.State,dest=>dest.MapFrom(src=>src.State.Trim()));
            CreateMap<Address, ResponseAddAddressDTO>();

            // get address
            CreateMap<Address, ResponseGetAddressDTO>();
            
            // make the address as default
            CreateMap<Address, ResponseMakeDefaultAddressDTO>();
        }
    }
}