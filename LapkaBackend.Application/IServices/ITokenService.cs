using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.IServices;

public interface ITokenService
{
    public string GenerateRefreshToken();
    public string GenerateAccessToken(User user);
    public Task<String> UseToken(string accessToken, string refreshToken, IDataContext context);
    public Task RevokeToken(string refreshToken, IDataContext context);
}