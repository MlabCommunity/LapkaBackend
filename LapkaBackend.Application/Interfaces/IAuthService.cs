using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;
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
        public Task RegisterUser(UserRegistrationRequest request);
        public Task<LoginResultDto> LoginUser(LoginRequest request);
        public Task RegisterShelter(ShelterWithUserRegistrationRequest request);
        public string CreateAccessToken(User user);
        public Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request);
        public string GenerateRefreshToken();
        public Task SaveRefreshToken(LoginRequest request, string tokens);
        public bool IsTokenValid(string token);
        public Task RevokeToken(TokenRequest request);
    }
}
