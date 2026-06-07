using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class RefundMappingProfile : Profile
    {
        public RefundMappingProfile()
        {
            CreateMap<RequestAddRefundDTO,Refund>();
            CreateMap<Refund,ResponseAddRefundDTO>();
        }
    }
}