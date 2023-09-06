using Google.Apis.Auth;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using System.Net.Http.Json;
using LapkaBackend.Application.Dtos.Responses;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace LapkaBackend.Application.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IDataContext _dbContext;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient = new();

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

    public async Task<LoginResultWithRoleDto> LoginUserByFacebook(FacebookRequest request)
    {
        try
        {
            var facebookUser = await _httpClient
                .GetFromJsonAsync<FacebookUserResponseDto>
                    ($"https://graph.facebook.com/{request.UserFbId}?fields=id,email,first_name,last_name,name,picture&access_token={request.FbAccessToken}&version=v15.0");

            if (facebookUser is null)
            {
                throw new BadRequestException("invalid_facebook_access_token", "Facebook access token is invalid.");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == facebookUser.Email);

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
                FirstName = facebookUser.FirstName,
                LastName = facebookUser.LastName,
                Email = facebookUser.Email,
                VerifiedAt = DateTime.UtcNow,
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
}