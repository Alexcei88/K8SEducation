using AutoMapper;
using OTUS.HomeWork.BillingService.Domain;

namespace OTUS.HomeWork.BillingService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {            
            CreateMap<PaymentDTO, Payment>().ReverseMap();

            CreateMap<UserDTO, User>()
                .ReverseMap();
        }
    }
}
