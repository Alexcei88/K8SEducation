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
                .ForMember(g => g.Refund, m => m.Ignore())
                .ForMember(g => g.IdempotanceKey, m => m.Ignore())
                .ForMember(g => g.Id, m => m.MapFrom(s => s.BillingId))
                .ReverseMap()
                .ForMember(g => g.IsSuccess, m => m.MapFrom(s => true));

            CreateMap<RefundDTO, Refund>()
                .ForMember(g => g.Date, m => m.Ignore())
                .ForMember(g => g.UserId, m => m.Ignore())
                .ForMember(g => g.Payment, m => m.Ignore())
                .ReverseMap()
                .ForMember(g => g.IsSuccess, m => m.MapFrom(s => true));
        }
    }
}
