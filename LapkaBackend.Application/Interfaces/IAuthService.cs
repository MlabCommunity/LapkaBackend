using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Interfaces
{
    public interface IAuthService
    {
         Task RegisterUser(UserRegistrationRequest request);
         Task<LoginResultDto> LoginShelter(LoginRequest request);
         Task<LoginResultDto> LoginUser(LoginMobileRequest request);
         Task RegisterShelter(ShelterWithUserRegistrationRequest request);
         string CreateAccessToken(User user);
         Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request);
         string CreateRefreshToken();
         bool IsTokenValid(string token);
         Task RevokeToken(TokenRequest request, Guid userId);
         Task ResetPassword(UserEmailRequest request);
         Task SetNewPassword(ResetPasswordRequest resetPasswordRequest, string token);
         Task ConfirmEmail(string token);
    }
}
