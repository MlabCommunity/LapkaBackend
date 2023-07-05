using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Entities;
using Microsoft.IdentityModel.Tokens;


namespace LapkaBackend.Application.Services;

public class UserService : IUserService
{
    public bool LoginMobile(IDataContext context, List<String> credentials)
    {
        var userDb = context.Users.FirstOrDefault(x => x.Email == credentials[0] && x.Password == credentials[1]);
        if (userDb == null) return false;

        userDb.RefreshToken = GenerateToken(DateTime.UtcNow.AddDays(3), "refresh_token");
        context.SaveChangesAsync();

        userDb.AccessToken = GenerateToken(DateTime.UtcNow.AddMinutes(5), "access_token");
        return true;
    }

    public bool LoginWeb(IDataContext context, List<String> credentials)
    {
        return true;
    }

    public bool Register(IDataContext context, List<String> credentials)
    {
        if (context.Users.Any(x => x.Email == credentials[0])) return false;
        if (credentials[1] != credentials[4]) return false;
        context.Users.Add(new User
        {
            FirstName = credentials[2],
            LastName = credentials[3],
            Email = credentials[0],
            Password = credentials[1],
            RefreshToken = GenerateToken(DateTime.UtcNow.AddDays(3), "refresh_token")
        });
        context.SaveChangesAsync();
        return true;
    }
    
    
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
    
    public string GenerateToken(DateTime expDate, string type)
    {
        
        var securityKey = new SymmetricSecurityKey("YhlyL1kqamyhR1Q4FBHrIjOOyd6rtajB"u8.ToArray()); 
        //Replace with getting the key more securely
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("token_type", type)
        };
        var token = new JwtSecurityToken("issuer",
            "audience",
            claims,
            expires: expDate,
            signingCredentials: credentials);
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }
    
}