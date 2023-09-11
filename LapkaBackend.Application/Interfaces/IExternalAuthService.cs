using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Interfaces;

public interface IExternalAuthService
{
    Task<LoginResultWithRoleDto> LoginUserByGoogle(string? tokenId);
    Task<LoginResultWithRoleDto> LoginUserByFacebook(FacebookRequest request);
}