using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.Interfaces
{
    public interface IAuthService
    {
        public Task<User> RegisterUser(UserRegisterDto user);
        public string LoginUser(UserLoginDto user);
        public string CreateToken(User user);
        public TokenDto GenerateRefreshToken();
        public Task SaveRefreshToken(UserLoginDto user, TokenDto tokens);
        public bool IsAccesTokenValid(string token);
        public Task RevokeToken(string token);
    }
}
