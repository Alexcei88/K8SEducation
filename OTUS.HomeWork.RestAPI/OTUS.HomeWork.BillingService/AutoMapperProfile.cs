using AutoMapper;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PaymentDTO, Payment>()
                .ForMember(g => g.User, m => m.Ignore())
                .ForMember(g => g.Date, m => m.Ignore())
                .ForMember(g => g.Amount, m => m.Ignore())
                .ForMember(g => g.UserId, m => m.Ignore())
                .ReverseMap();

            CreateMap<UserDTO, User>()
               .ForMember(g => g.Payments, m => m.Ignore())
               .ForMember(g => g.Id, m => m.MapFrom(s => s.UserId))
               .ReverseMap();
        }
    }
}
