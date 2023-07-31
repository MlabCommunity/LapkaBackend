using Google.Apis.Auth;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using LapkaBackend.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LapkaBackend.Application.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IDataContext _dbContext;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    
    public ExternalAuthService(IDataContext dbContext, IAuthService authService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _authService = authService;
        _configuration = configuration;
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
                user.RefreshToken = _authService.CreateRefreshToken();
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
                RefreshToken = _authService.CreateRefreshToken(),
                Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == Roles.User.ToString())!
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
        bool isTokenValid = await ValidateFacebookAccessToken(userFbId!, fbAccessToken!);
        if (!isTokenValid)
        {
            throw new BadRequestException("invalid_facebook_access_token", "Facebook access token is invalid.");
        }
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(fbAccessToken);
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == jwtToken.Claims.FirstOrDefault(x => x.Type == "email")!.Value);
        if (user != null)
        {
            user.RefreshToken = _authService.CreateRefreshToken();
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return new LoginResultWithRoleDto
            {
                AccessToken = _authService.CreateAccessToken(user),
                RefreshToken = user.RefreshToken,
                Role = user.Role!.RoleName
            };
        }
        var newFacebookUser = new User
        {
            FirstName = jwtToken.Claims.FirstOrDefault(x => x.Type == "first_name")!.Value,
            LastName = jwtToken.Claims.FirstOrDefault(x => x.Type == "last_name")!.Value,
            Email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")!.Value,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = _authService.CreateRefreshToken(),
            Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == Roles.User.ToString())!
        };
        _dbContext.Users.Add(newFacebookUser);
        await _dbContext.SaveChangesAsync();
        return new LoginResultWithRoleDto
        {
            AccessToken = _authService.CreateAccessToken(newFacebookUser),
            RefreshToken = newFacebookUser.RefreshToken,
            Role = newFacebookUser.Role.RoleName
        };
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

        var foundUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == emailUser);

        if (!(foundUser is null))
        {
            return new LoginResultWithRoleDto()
            {
                AccessToken = _authService.CreateAccessToken(foundUser),
                RefreshToken = _authService.CreateRefreshToken(),
                Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == Roles.User.ToString())!.RoleName
            };
        }

        var newAppleUser = new User
        {
            FirstName = firstName!,
            LastName = lastName!,
            Email = emailUser,
            CreatedAt = DateTime.UtcNow,
            RefreshToken = _authService.CreateRefreshToken(),
            Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == Roles.User.ToString())!
        };

        await _dbContext.Users.AddAsync(newAppleUser);
        await _dbContext.SaveChangesAsync();

        return new LoginResultWithRoleDto()
        {
            AccessToken = _authService.CreateAccessToken(newAppleUser),
            RefreshToken = newAppleUser.RefreshToken,
            Role = _dbContext.Roles.FirstOrDefault(r => r.RoleName == Roles.User.ToString())!.RoleName
        };
    }
    
    
    private async Task<bool> ValidateFacebookAccessToken(string accessToken, string userId)
    {
        // Make an API call to Facebook to validate the access token
        var httpClient = new HttpClient();
        var appId = _configuration["Facebook:AppId"];
        string appSecret = _configuration["Facebook:AppSecret"]!;
        string debugTokenUrl = $"https://graph.facebook.com/v15.0/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";

        HttpResponseMessage response = await httpClient.GetAsync(debugTokenUrl);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseContent);
            return result?.Data.IsValid == true && result.Data.UserId == userId;
        }

        return false;
    }
}

public class FacebookTokenValidationResult
{
    public FacebookTokenValidationData Data { get; set; } = null!;
}

public class FacebookTokenValidationData
{
    public string UserId { get; set; } = null!;
    public bool IsValid { get; set; }
}