using Google.Apis.Auth;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace LapkaBackend.Application.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IDataContext _dbContext;
    private readonly IAuthService _authService;
    
    public ExternalAuthService(IDataContext dbContext, IAuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }
    public async Task<LoginResultWithRoleDto> LoginUserByGoogle(string? tokenId)
    {
        // Check if token exists
        if (tokenId == null)
        {
            throw new BadRequestException("invalid_google_id_token", "Google token is invalid.");
        }
        try
        {
            // Validate token if token not valid exception will be thrown
            var payload = GoogleJsonWebSignature.ValidateAsync(tokenId, new GoogleJsonWebSignature.ValidationSettings())
                .Result;
            // Check if user exists
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == payload.Email);
            if (user != null)
            {
                user.RefreshToken = _authService.GenerateRefreshToken();
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return new LoginResultWithRoleDto
                {
                    AccessToken = _authService.CreateAccessToken(user),
                    RefreshToken = user.RefreshToken,
                    Role = user.Role!.RoleName
                };
            }
            // If user not exists create new user
            var newGoogleUser = new User
            {
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Email = payload.Email,
                CreatedAt = DateTime.UtcNow,
                RefreshToken = _authService.GenerateRefreshToken(),
                Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "User")!
            };
            _dbContext.Users.Add(newGoogleUser);
            await _dbContext.SaveChangesAsync();

            return new LoginResultWithRoleDto
            {
                AccessToken = _authService.CreateAccessToken(newGoogleUser),
                RefreshToken = newGoogleUser.RefreshToken,
                Role = newGoogleUser.Role.RoleName
            };
        }
        catch (InvalidJwtException)
        {
            throw new BadRequestException("invalid_google_id_token", "Google token is invalid.");
        }
        
    }
    
    public async Task<LoginResultWithRoleDto> LoginUserByFacebook(string? userFbId, string? fbAccessToken)
    {
        throw new NotImplementedException();
    }
    

    public async Task<LoginResultWithRoleDto> LoginUserByApple(string? appleAccessToken, string? firstName, string? lastName)
    {
        // Validate and parse the access token.
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(appleAccessToken))
        {
            throw new BadRequestException("invalid_apple_token", "Invalid Access Token");
        }

        // Read the claims from the access token.
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(appleAccessToken);

        string emailUser = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")!.Value;

        var findedUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == emailUser);

        if (!(findedUser is null))
        {
            return new LoginResultWithRoleDto()
            {
                AccessToken = _authService.CreateAccessToken(findedUser),
                RefreshToken = _authService.GenerateRefreshToken(),
                Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "User")!.RoleName
            };
        }

        var newAppleUser = new User
        {
            FirstName = firstName!,
            LastName = lastName!,
            Email = emailUser,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = _authService.GenerateRefreshToken(),
            Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "User")!
        };

        await _dbContext.Users.AddAsync(newAppleUser);
        await _dbContext.SaveChangesAsync();

        return new LoginResultWithRoleDto()
        {
            AccessToken = _authService.CreateAccessToken(newAppleUser),
            RefreshToken = newAppleUser.RefreshToken,
            Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "User")!.RoleName
        };
    }
}