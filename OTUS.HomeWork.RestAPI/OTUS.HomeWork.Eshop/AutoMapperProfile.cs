using AutoMapper;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.Domain;

namespace OTUS.HomeWork.Eshop
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {            
            CreateMap<ProductDTO, Product>().ReverseMap();

            CreateMap<CreateOrderDTO, Order>()
                .ForMember(g => g.PaidDateUtc, m => m.Ignore())
                .ForMember(g => g.TotalPrice, m => m.Ignore())
                .ForMember(g => g.BillingId, m => m.Ignore())
                .ForMember(g => g.PaidDateUtc, m => m.Ignore())
                .ForMember(g => g.CreatedOnUtc, m => m.Ignore())
                .ForMember(g => g.OrderNumber, m => m.Ignore())
                .ForMember(g => g.UserId, m => m.Ignore())
                .ForMember(g => g.Status, m => m.Ignore())
                .ReverseMap();

            CreateMap<OrderItemDTO, OrderItem>()
                .ForMember(g => g.Order, m => m.Ignore())
                .ForMember(g => g.OrderNumberId, m => m.Ignore())
                .ReverseMap();

            CreateMap<Order, CreatedOrderDTO>();
        }
    }
}
