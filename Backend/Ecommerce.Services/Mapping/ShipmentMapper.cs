using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public class ShipmentMappingProfile : Profile
    {
        public ShipmentMappingProfile()
        {
            CreateMap<RequestAddShipmentDTO,Shipment>();
            CreateMap<Shipment,ResponseAddShipmentDTO>();

            CreateMap<ShipmentTracking,ResponseAddShipmentTrackingDTO>();
            CreateMap<RequestAddShipmentDTO,ShipmentTracking>();
        }
    }
}