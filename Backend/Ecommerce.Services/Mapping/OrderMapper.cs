using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<RequestCreateOrderDTO,Order>();
            CreateMap<Payment,ResponsePayment>();

            CreateMap<RequestAddReturnDTO,Return>();
            CreateMap<Return,ResponseAddReturnDTO>();
        }
    }
}