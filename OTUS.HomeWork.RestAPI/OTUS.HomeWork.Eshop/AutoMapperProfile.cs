using AutoMapper;

namespace OTUS.HomeWork.Eshop
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            /*
            CreateMap<User, UserDTO>()
                .ForMember(g => g.UserId, m => m.MapFrom(s => s.Id))
                .ReverseMap();
                */
        }
    }
}
