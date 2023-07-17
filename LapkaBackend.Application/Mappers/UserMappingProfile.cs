using AutoMapper;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
