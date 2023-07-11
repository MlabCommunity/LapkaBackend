using Azure.Core;
using LapkaBackend.Application.ApplicationDtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILapkaBackendDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private string? secretKey;

        public AuthService(IConfiguration configuration, ILapkaBackendDbContext dbContext, string? secretKey)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            this.secretKey = secretKey;
        }

        

        public async Task<ActionResult<User>> UserRegister(UserDto userDto)
        {
            if (!string.IsNullOrWhiteSpace(userDto.FirstName) && !string.IsNullOrWhiteSpace(userDto.LastName) && !string.IsNullOrWhiteSpace(userDto.EmailAddress) && !string.IsNullOrWhiteSpace(userDto.Password) && !string.IsNullOrWhiteSpace(userDto.ConfirmPassword))
            {
                if (userDto.Password == userDto.ConfirmPassword)
                {
                    CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    DateTime dateTimeNow = DateTime.Now;
                    string createdAt = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");

                    var user = new User();
                    user.FirstName = userDto.FirstName;
                    user.LastName = userDto.LastName;
                    user.Email = userDto.EmailAddress;
                    user.CreatedAt = createdAt;
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.RefreshToken = GenerateRefreshToken(user, secretKey, 15);

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();

                    return new OkObjectResult(user);
                }

                return new BadRequestObjectResult("Passwords don't match");
            }
            return new BadRequestObjectResult("Complete the required fields");
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<ActionResult<Shelter>> ShelterRegister(ShelterDto shelterDto)
        {
            if (!(string.IsNullOrWhiteSpace(shelterDto.City) || string.IsNullOrWhiteSpace(shelterDto.Krs) || string.IsNullOrWhiteSpace(shelterDto.Nip) || string.IsNullOrWhiteSpace(shelterDto.OrganizationName) || string.IsNullOrWhiteSpace(shelterDto.PhoneNumber) || string.IsNullOrWhiteSpace(shelterDto.Street) || string.IsNullOrWhiteSpace(shelterDto.ZipCode)))
            {
                var shelter = new Shelter();
                shelter.OrganizationName = shelterDto.OrganizationName;
                shelter.Longtitude = shelterDto.Longtitude;
                shelter.Latitude = shelterDto.Latitude;
                shelter.City = shelterDto.City;
                shelter.Street = shelterDto.Street;
                shelter.ZipCode = shelterDto.ZipCode;
                shelter.Nip = shelterDto.Nip;
                shelter.Krs = shelterDto.Krs;
                shelter.PhoneNumber = shelterDto.PhoneNumber;

                _dbContext.Shelters.Add(shelter);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(shelter);
            }
            return new BadRequestObjectResult("Complete the required fields");
        }
        
        public async Task<ActionResult<TokenResponse>> Login(LoginUserDto loginUserDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);

            if (user == null)
            {
                return new BadRequestObjectResult("User not found.");
            }

            if (!VerifyPasswordHash(loginUserDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new BadRequestObjectResult("Wrong password.");
            }

            string refreshToken;
            if (!IsTokenValid(user.RefreshToken))
            {
                refreshToken = GenerateRefreshToken(user, secretKey, 15);

                user.RefreshToken = refreshToken;
                await _dbContext.SaveChangesAsync();               
            }
            else
            {
                refreshToken = user.RefreshToken;
            }

            string accessToken = GenerateAccessToken(user, secretKey, 60);
            
            var tokenResponse = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return new OkObjectResult(tokenResponse);
        }
        
        private bool IsTokenValid(string tokenString)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);
            if (token.Payload.TryGetValue("Expiration", out var expirationClaimValue) && expirationClaimValue is string expirationString)
            {
                if (DateTime.TryParse(expirationString, out var expirationDate))
                {
                    return DateTime.UtcNow >= expirationDate;
                }
                    throw new ArgumentException("Invalid expiration date format");
            }
            else if (expirationClaimValue is DateTime expirationDate)
            {
                return DateTime.UtcNow >= expirationDate;
            }
                throw new ArgumentException("Expiration claim value is not a valid date");
        } 

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateAccessToken(User user, string secretKey, int expiryMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private string GenerateRefreshToken(User user, string secretKey, int expiryDays)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim("Expiration", DateTime.UtcNow.AddDays(expiryDays).ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(expiryDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        
    }
}
