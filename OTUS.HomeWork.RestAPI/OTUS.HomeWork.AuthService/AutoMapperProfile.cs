using AutoMapper;
using OTUS.HomeWork.AuthService.Domain;
using User = OTUS.HomeWork.RestAPI.Abstraction.Domain.User;

namespace OTUS.HomeWork.AuthService
{
    public class AutoMapperProfile
        : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterUserDTO, User>()
                .ForMember(g => g.Id, m => m.Ignore());
            
            CreateMap<User, UserDTO>()
                .ForMember(g => g.UserId, 
                    m => m.MapFrom(s => s.Id))
                .ReverseMap();
        }
    }
}
