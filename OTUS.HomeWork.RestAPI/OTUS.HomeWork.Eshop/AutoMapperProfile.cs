using AutoMapper;
using OTUS.HomeWork.EShop.Domain;
using OTUS.HomeWork.EShop.Domain.DTO;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;
using System.Linq;

namespace OTUS.HomeWork.EShop
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
                .ForMember(g => g.ErrorDescription, m => m.Ignore())
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

            CreateMap<Bucket[], BucketRequestDTO>()
                .ForMember(g => g.Items, m => m.MapFrom(s => s.Select(g => new OrderItemDTO
                {
                    ProductId = g.ProductId,
                    Quantity = g.Quantity
                }).ToList()));

        }
    }
}
