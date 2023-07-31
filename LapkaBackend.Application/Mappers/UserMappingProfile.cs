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

    public class ShelterInListMappingProfile : Profile
    {
        public ShelterInListMappingProfile()
        {
            CreateMap<Shelter, ShelterInListDto>().ReverseMap();
        }

    }

    /*
    public class PetInListMappingProfile : Profile
    {
        public PetInListMappingProfile()
        {
            CreateMap<Animal, PetInListDto>()
                .ForMember(a => a.Color, c => c.MapFrom(c => c.Marking))
                .ForMember(a => a.ProfilePhoto, c => c.MapFrom(c => c.Species))
                .ForMember(a => a.Photos, c => c.MapFrom(c => c.Species))
                //.ForMember(a => a.TotalPages,c=>c.MapFrom)
            }
                 //.ForMember(a=>a.TotalPages)
                 //.ForMember(a=>a.TotalItemsCount)



             } 

    } */

}
