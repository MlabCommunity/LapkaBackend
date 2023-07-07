using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Consts;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LapkaBackend.Application.Services;

public class TokenService : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User"),
        };
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: "issuer",
            audience: "audience",
            expires: AccessTokenConsts.GetExpDate(),
            signingCredentials: GetCredentials());
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }
    public string GenerateRefreshToken()
    {
        var token = new JwtSecurityToken(
            expires: RefreshTokenConsts.GetExpDate(),
            signingCredentials: GetCredentials());
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }

    public async Task<string> UseToken(string accessToken, string refreshToken, IDataContext context)
    {
        if (!ValidateToken(refreshToken)) throw new PlaceholderException("Token is not valid");
        if (ValidateToken(accessToken)) return accessToken;
        var jwtAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var emailFromToken = jwtAccessToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == emailFromToken);
        if (user == default) throw new PlaceholderException("User not found");
        user.AccessToken = GenerateAccessToken(user);
        return user.AccessToken;
    }
    
    public async Task RevokeToken(string refreshToken, IDataContext context)
    {
        if (!ValidateToken(refreshToken)) throw new PlaceholderException("Token is not valid");
        var user = await context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user == default) throw new PlaceholderException("User not found");
        user.RefreshToken = null;
        await context.SaveChangesAsync();
    }

    
    private bool ValidateToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
    
    private SigningCredentials GetCredentials()
    {
        var securityKey = new SymmetricSecurityKey("YhlyL1kqamyhR1Q4FBHrIjOOyd6rtajB"u8.ToArray()); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return credentials;
    }
}