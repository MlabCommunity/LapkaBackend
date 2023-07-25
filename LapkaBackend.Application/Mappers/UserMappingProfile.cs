using AutoMapper;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Functions.Queries;
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

    public class ShelterMappingProfile:Profile
    {
        public ShelterMappingProfile()
        {
            CreateMap<Shelter, ShelterDto>().ReverseMap();
        }

    }
}
