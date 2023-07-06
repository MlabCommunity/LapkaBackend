using System.IdentityModel.Tokens.Jwt;
using LapkaBackend.Application.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LapkaBackend.Application.Services;

public class TokenService : ITokenService
{
    public string GenerateAccessToken()
    {
        
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: GetCredentials());
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }
    public string GenerateRefreshToken()
    {
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: GetCredentials());
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }
    
    public bool ValidateToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
    
    public async Task<string> UseToken(string accessToken, string refreshToken, IDataContext context)
    {
        if (!ValidateToken(refreshToken)) return null;
        if (ValidateToken(accessToken)) return accessToken;
        var user = await context.Users.FirstAsync(x => x.RefreshToken == refreshToken);
        if (user == null) throw new Exception(GenerateAccessToken());
        user.AccessToken = GenerateAccessToken();
        return user.AccessToken;
    }

    private SigningCredentials GetCredentials()
    {
        var securityKey = new SymmetricSecurityKey("YhlyL1kqamyhR1Q4FBHrIjOOyd6rtajB"u8.ToArray()); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return credentials;
    }
}