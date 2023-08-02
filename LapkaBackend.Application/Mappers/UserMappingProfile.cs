using AutoMapper;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;

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

    public class ShelterInListMappingProfile : Profile
    {
        public ShelterInListMappingProfile()
        {
            CreateMap<Shelter, ShelterInListDto>().ReverseMap();
        }

    }


}
