using AutoMapper;
using OTUS.HomeWork.RestAPI.Domain;

namespace OTUS.HomeWork.RestAPI
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(g => g.UserId, m => m.MapFrom(s => s.Id))
                .ReverseMap();
        }
    }
}
