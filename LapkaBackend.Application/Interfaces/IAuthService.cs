using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Interfaces
{
    public interface IAuthService
    {
        public Task RegisterUser(UserRegisterDto user);
        public Task<LoginResultDto> LoginUser(UserLoginDto user);
        public string CreateAccessToken(User user);
        public string GenerateRefreshToken();
        public Task SaveRefreshToken(UserLoginDto user, string tokens);
        public bool IsTokenValid(string token);
        public Task RevokeToken(string token);
    }
}
