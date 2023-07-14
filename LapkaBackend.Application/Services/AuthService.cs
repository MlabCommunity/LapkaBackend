using Azure.Core;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LapkaBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(IDataContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        
        public async Task RegisterUser(UserRegistrationRequest request)
        {
            
            if (request.Password != request.ConfirmPassword)
            {
                throw new AuthException("Passwords do not match");
            }

            var newUser = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.Now
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LoginResultDto> LoginUser(LoginRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new AuthException("User not found");
            }

            if (result.Password != request.Password)
            {
                throw new AuthException("Wrong password");
            }
            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : GenerateRefreshToken()
            }; 
        }

        public async Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request) 
        {
            // TODO: (Najważniejsze) dodać metode refreshaccesstoken ma przyjmować tokeny a create usera 
            // TODO: Do przeanalizowania struktura

            var jwtAccesToken = new JwtSecurityToken(request.AccessToken);

            if (jwtAccesToken == null)
            {
                throw new AuthException("Błędny token");
            }

            jwtAccesToken.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value);

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == jwtAccesToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value);

            if(user == null)
            {
                throw new AuthException("Nie znaleziono użytkownika");
            }

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, "User")
                // TODO: Change Admin to user.Role 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new UseRefreshTokenResultDto { AccessToken = jwt};
        }

        public string CreateAccessToken(User user)
        { 

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, "User")
                // TODO: Change Admin to user.Role 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public string GenerateRefreshToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task SaveRefreshToken(LoginRequest request, string newRefreshToken)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if(result is null) 
            {
                throw new AuthException("User not found");
            }
            
            result.RefreshToken = newRefreshToken;

            _dbContext.Users.Update(result);

            await _dbContext.SaveChangesAsync();
        }

        public bool IsTokenValid(string token)
        {
            JwtSecurityToken jwtSecurityToken;
            try
            {
                jwtSecurityToken = new JwtSecurityToken(token);
                
            }
            catch (Exception)
            {
                return false;
            }
            return jwtSecurityToken.ValidTo > DateTime.UtcNow;
        }

        public async Task RevokeToken(TokenRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x=> x.RefreshToken == request.RefreshToken);

            if (result is not null)
            {
                result.RefreshToken = "";
                _dbContext.Users.Update(result);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RegisterShelter(ShelterWithUserRegistrationRequest request)
        {
            var newShelter = new Shelter()
            {
                OrganizationName = request.ShelterRequest.OrganizationName,
                Longtitude = request.ShelterRequest.Longitude,
                Latitude = request.ShelterRequest.Latitude,
                City = request.ShelterRequest.City,
                Street = request.ShelterRequest.Street,
                ZipCode = request.ShelterRequest.ZipCode,
                Nip = request.ShelterRequest.Nip,
                Krs = request.ShelterRequest.Krs,
                PhoneNumber = request.ShelterRequest.PhoneNumber,
            };

            await _dbContext.Shelters.AddAsync(newShelter);

            var newUser = new User()
            {
                FirstName = request.UserRequest.FirstName,
                LastName = request.UserRequest.LastName,
                Email = request.UserRequest.Email,
                Password = request.UserRequest.Password,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.Now,
                ShelterId = newShelter.Id
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}
