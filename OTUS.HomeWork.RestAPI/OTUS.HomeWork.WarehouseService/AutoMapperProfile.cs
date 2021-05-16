using AutoMapper;
using OTUS.HomeWork.WarehouseService.Contract.DTO;
using OTUS.HomeWork.WarehouseService.Domain;

namespace OTUS.HomeWork.WarehouseService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductDTO, Product>()
                .ForMember(g => g.Weight, m => m.Ignore())
                .ForMember(g => g.Space, m => m.Ignore())
                .ReverseMap()
                .ForMember(g => g.RemainCount, m => m.Ignore());

            CreateMap<ReserveProduct, ReserveProductDTO>()
                .ForMember(g => g.Id, m => m.MapFrom(s => s.ProductId))
                .ReverseMap()
                .ForMember(g => g.ProductId, m => m.MapFrom(s => s.Id))
                .ForMember(g => g.OrderNumber, m => m.Ignore());
        }
    }
}
