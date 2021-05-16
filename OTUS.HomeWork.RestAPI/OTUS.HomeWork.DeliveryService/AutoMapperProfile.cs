using AutoMapper;
using OTUS.HomeWork.DeliveryService.Contract.DTO;
using OTUS.HomeWork.DeliveryService.Contract.Messages;
using OTUS.HomeWork.DeliveryService.Domain;

namespace OTUS.HomeWork.DeliveryService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DeliveryOrderRequest, Delivery>()
                .ForMember(g => g.Location, m => m.MapFrom(g => new DeliveryLocation()
                {
                    OrderNumber = g.OrderNumber
                }))
                .ForMember(g => g.CourierName, m => m.Ignore());

            CreateMap<Delivery, DeliveryResponseDTO>()
                .ForMember(g => g.EstimatedDate, m => m.MapFrom(s => s.Location.EstimatedDate))
                .ForMember(g => g.ShipmentDate, m => m.MapFrom(s => s.Location.ShipmentDate))
                .ForMember(g => g.DeliveryAddress, m => m.MapFrom(s => s.Location.EstimatedDate));

            CreateMap<DeliveryLocation, DeliveryLocationDTO>()
                .ForMember(g => g.Address, m => m.MapFrom(s => s.CurrentAddress))
                .ForMember(g => g.DeliveryDate, m => m.MapFrom(s => s.EstimatedDate));

            CreateMap<DeliveryOrderRequest.DeliveryProduct, DeliveryProduct>()
                .ForMember(g => g.OrderNumber, m => m.Ignore())
                .ForMember(g => g.Delivery, m => m.Ignore())
                .ForMember(g => g.Name, m => m.Ignore());
        }
    }
}
