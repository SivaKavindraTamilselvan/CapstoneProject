using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class RefundMappingProfile : Profile
    {
        public RefundMappingProfile()
        {
            CreateMap<RequestAddReturnRefundDTO,Refund>();
            CreateMap<Refund,ResponseAddRefundDTO>();

            CreateMap<RequestAddReturnRefundDTO,ReturnRefund>();
        }
    }
}