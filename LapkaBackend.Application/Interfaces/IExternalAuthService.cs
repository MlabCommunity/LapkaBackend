using LapkaBackend.Application.Dtos.Result;

namespace LapkaBackend.Application.Interfaces;

public interface IExternalAuthService
{
    public Task<LoginResultWithRoleDto> LoginUserByGoogle(string? tokenId);
    public Task<LoginResultWithRoleDto> LoginUserByFacebook(string? userFbId, string? fbAccessToken);
    public Task<LoginResultWithRoleDto> LoginUserByApple(string? appleAccessToken, string? firstName, string? lastName);
}