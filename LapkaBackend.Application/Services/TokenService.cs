using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Consts;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LapkaBackend.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IDataContext _context;
    public TokenService(IConfiguration configuration, IDataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

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

    public async Task<string> UseToken(string accessToken, string refreshToken)
    {
        if (!ValidateToken(refreshToken)) 
            throw new PlaceholderException("Token is not valid");
        if (ValidateToken(accessToken)) 
            return accessToken;
        var jwtAccessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var emailFromToken = jwtAccessToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == emailFromToken);
        if (user is null) 
            throw new PlaceholderException("User not found");
        user.AccessToken = GenerateAccessToken(user);
        return user.AccessToken;
    }
    
    public async Task RevokeToken(string refreshToken)
    {
        if (!ValidateToken(refreshToken)) 
            throw new PlaceholderException("Token is not valid");
        var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (user is null) 
            throw new PlaceholderException("User not found");
        user.RefreshToken = null;
        await _context.SaveChangesAsync();
    }

    
    private bool ValidateToken(string? token)
    {
        if (string.IsNullOrEmpty(token)) 
            return false;
        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
    
    private SigningCredentials GetCredentials()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return credentials;
    }
}