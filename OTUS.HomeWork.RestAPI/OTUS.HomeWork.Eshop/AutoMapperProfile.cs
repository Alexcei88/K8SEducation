using AutoMapper;
using OTUS.HomeWork.AuthService.Domain;
using OTUS.HomeWork.Eshop.Domain;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;

namespace OTUS.HomeWork.Eshop
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {            
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

            CreateMap<RegisterUserDTO, User>()
               .ForMember(g => g.Id, m => m.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(g => g.UserId,
                    m => m.MapFrom(s => s.Id))
                .ReverseMap();
        }
    }
}
