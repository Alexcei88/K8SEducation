using AutoMapper;
using OTUS.HomeWork.NotificationService.Domain;

namespace OTUS.HomeWork.Eshop
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NotificationDTO, Notification>()
                .ForMember(s => s.Id, m => m.Ignore())
                .ForMember(s => s.UserId, m => m.Ignore())
                .ReverseMap();
                
        }
    }
}
