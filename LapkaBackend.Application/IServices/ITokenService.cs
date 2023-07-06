namespace LapkaBackend.Application.IServices;

public interface ITokenService
{
    public string GenerateRefreshToken();
    public string GenerateAccessToken();
    public bool ValidateToken(string token);
    public Task<String> UseToken(string accessToken, string refreshToken, IDataContext context);
}