using AutoMapper;
using OTUS.HomeWork.PaymentGatewayService.Domain;

namespace OTUS.HomeWork.PaymentGatewayService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PaymentDTO, Payment>()
                .ForMember(g => g.Date, m => m.Ignore())
                .ForMember(g => g.Amount, m => m.Ignore())
                .ForMember(g => g.UserId, m => m.Ignore())
                .ForMember(g => g.IdempotanceKey, m => m.Ignore())
                .ForMember(g => g.Id, m => m.Ignore())
                .ReverseMap();
        }
    }
}
