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
        public Task<User?> RegisterUser(UserRegisterDto user);
        public Task<LoginResultDto> LoginUser(UserLoginDto user);
        public string CreateToken(User user);
        public string GenerateRefreshToken();
        public Task SaveRefreshToken(UserLoginDto user, string tokens);
        public bool IsAccesTokenValid(string token);
        public Task RevokeToken(string token);
    }
}
