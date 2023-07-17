using AutoMapper;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace LapkaBackend.Application.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            //.ForMember(m => m.Id, c => c.MapFrom(n => n.Id))
            //.ForMember(m => m.FirstName, c => c.MapFrom(n => n.FirstName))
            //.ForMember(m => m.LastName, c => c.MapFrom(n => n.LastName))
            //.ForMember(m => m.Email, c => c.MapFrom(n => n.Email))
            //.ForMember(m => m.CreatedAt, c => c.MapFrom(n => n.CreatedAt));


        }

    }
}
