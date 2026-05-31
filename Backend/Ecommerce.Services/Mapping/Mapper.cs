using AutoMapper;
using Ecommerce.Models;
using Ecommerce.DTOs;

namespace Ecommerce.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestRegisterUserDTO, User>();
            CreateMap<User,ResponseRegisterUserDTO>();

            CreateMap<RequestLoginUserDTO, User>();
        }
    }
}