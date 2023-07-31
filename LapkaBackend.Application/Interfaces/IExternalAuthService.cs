using LapkaBackend.Application.Dtos.Result;

namespace LapkaBackend.Application.Interfaces;

public interface IExternalAuthService
{
    Task<LoginResultWithRoleDto> LoginUserByGoogle(string? tokenId);
    Task<LoginResultWithRoleDto> LoginUserByFacebook(string? userFbId, string? fbAccessToken);
    Task<LoginResultWithRoleDto> LoginUserByApple(string? appleAccessToken, string? firstName, string? lastName);
}