using LapkaBackend.Application.ApplicationDtos;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ActionResult<User>> UserRegister(UserDto userDto);
        public Task<ActionResult<Shelter>> ShelterRegister(ShelterDto shelterDto);
        public Task<ActionResult<TokenResponse>> Login(LoginUserDto loginUserDto);
    }
}
