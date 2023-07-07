using LapkaBackend.Application.ApplicationDtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        public AuthService(IConfiguration configuration, ILapkaBackendDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
            public async Task<ActionResult<User>> UserRegister(UserDto userDto)
        {
            if (!(string.IsNullOrWhiteSpace(userDto.firstName) && string.IsNullOrWhiteSpace(userDto.lastName) && string.IsNullOrWhiteSpace(userDto.emailAddress) && string.IsNullOrWhiteSpace(userDto.password) && string.IsNullOrWhiteSpace(userDto.confirmPassword)))
            {
                if (userDto.password == userDto.confirmPassword)
                {
                    CreatePasswordHash(userDto.password, out byte[] passwordHash, out byte[] passwordSalt);

                    DateTime dateTimeNow = DateTime.Now;
                    string createdAt = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");

                    var user = new User();
                    user.FirstName = userDto.firstName;
                    user.LastName = userDto.lastName;
                    user.Email = userDto.emailAddress;
                    user.CreatedAt = createdAt;
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

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

        // Rejestracja schroniska
        public async Task<ActionResult<Shelter>> ShelterRegister(ShelterDto shelterDto)
        {
            if (!(string.IsNullOrWhiteSpace(shelterDto.City) && string.IsNullOrWhiteSpace(shelterDto.Krs) && string.IsNullOrWhiteSpace(shelterDto.Nip) && string.IsNullOrWhiteSpace(shelterDto.OrganizationName) && string.IsNullOrWhiteSpace(shelterDto.phoneNumber) && string.IsNullOrWhiteSpace(shelterDto.Street) && string.IsNullOrWhiteSpace(shelterDto.ZipCode)))
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
                shelter.phoneNumber = shelterDto.phoneNumber;

                _dbContext.Shelters.Add(shelter);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(shelter);
            }
            return new NoContentResult();
        }

        /*
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)); // tu błąd System.ArgumentOutOfRangeException: IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'http://www.w3.org/2001/04/xmldsig-more#hmac-sha512', the key size must be greater than: '512' bits, key has '136' bits. (Parameter 'keyBytes')

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }   */

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

            string secretKey = "secret-key-secret-key-secret-key";
            int accessTokenExpiryMinutes = 60;
            string accessToken = GenerateAccessToken(user,secretKey,accessTokenExpiryMinutes);
            string refreshToken = GenerateRefreshToken();

            var tokenResponse = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return new OkObjectResult(tokenResponse);
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string GenerateAccessToken(User user, string secretKey, int expiryMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            // można dodać inne potrzebne claimy
        }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }



    }
}
