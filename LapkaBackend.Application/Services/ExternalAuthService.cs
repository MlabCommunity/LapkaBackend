using Google.Apis.Auth;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using LapkaBackend.Application.Dtos.Responses;
using LapkaBackend.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LapkaBackend.Application.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IDataContext _dbContext;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient = new ();
    
    public ExternalAuthService(IDataContext dbContext, IAuthService authService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _authService = authService;
        _configuration = configuration;
    }
    public async Task<LoginResultWithRoleDto> LoginUserByGoogle(string? tokenId)
    {
        if (tokenId == null)
        {
            throw new BadRequestException("invalid_google_id_token", "Google token is invalid.");
        }
        
        try
        {
            var googleUser = await _httpClient
                .GetFromJsonAsync<GoogleUserResponseDto>($"https://oauth2.googleapis.com/tokeninfo?id_token={tokenId}");
            
            if (googleUser is null)
            {
                throw new BadRequestException("invalid_google_id_token", "Google token is invalid.");
            }
            
            // Check if user exists
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == googleUser.Email);
            if (user != null)
            {
                user.RefreshToken = _authService.CreateRefreshToken();
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                return new LoginResultWithRoleDto
                {
                    AccessToken = _authService.CreateAccessToken(user),
                    RefreshToken = user.RefreshToken,
                    Role = user.Role.RoleName
                };
            }
            // If user not exists create new user
            var newGoogleUser = new User
            {
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                Email = googleUser.Email,
                VerifiedAt = googleUser.EmailVerified ? DateTime.UtcNow : null,
                Password = "temporary_password",
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
        var isTokenValid = await ValidateFacebookAccessToken(userFbId!, fbAccessToken!);
        
        if (!isTokenValid)
        {
            throw new BadRequestException("invalid_facebook_access_token", "Facebook access token is invalid.");
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(fbAccessToken);
        
        var user = _dbContext.Users.FirstOrDefault(u => 
            u.Email == jwtToken.Claims.FirstOrDefault(x => x.Type == "email")!.Value);
        
        if (user != null)
        {
            user.RefreshToken = _authService.CreateRefreshToken();
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return new LoginResultWithRoleDto
            {
                AccessToken = _authService.CreateAccessToken(user),
                RefreshToken = user.RefreshToken,
                Role = user.Role.RoleName
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
        var appSecret = _configuration["Facebook:AppSecret"]!;
        var debugTokenUrl = $"https://graph.facebook.com/v15.0/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";

        var response = await httpClient.GetAsync(debugTokenUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return false;
        var result = JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseContent);
        return result?.Data.IsValid == true && result.Data.UserId == userId;

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