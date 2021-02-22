using AutoMapper;
using OTUS.HomeWork.AuthService.Domain;
using OTUS.HomeWork.RestAPI.Domain;
using OTUS.HomeWork.UserService.Domain;

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
