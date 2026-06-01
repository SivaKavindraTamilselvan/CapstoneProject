using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestRegisterUserDTO, User>().ForMember(dest => dest.Password, opt => opt.Ignore()).ForMember(dest => dest.HashedKey, opt => opt.Ignore());
            CreateMap<User,ResponseRegisterUserDTO>();

            CreateMap<RequestLoginUserDTO, User>();
        }
    }
}