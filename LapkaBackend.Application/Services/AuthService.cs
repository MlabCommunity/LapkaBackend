using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
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
        
        public async Task RegisterUser(UserRegisterDto user)
        {
            if (user.Password != user.ConfirmPassword)
            {
                throw new AuthException("Passwords do not match");
            }

            var newUser = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.Now
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LoginResultDto> LoginUser(UserLoginDto user)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            if (result == null)
            {
                throw new AuthException("User not found");
            }

            if (result.Password != user.Password)
            {
                throw new AuthException("Wrong password");
            }
            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = GenerateRefreshToken()
            }; 
        }

        public string CreateAccessToken(User user) 
        {
            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Email),
                // new (ClaimTypes.Role, "Admin") 
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

        public async Task SaveRefreshToken(UserLoginDto user, string newRefreshToken)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

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

        public async Task RevokeToken(string token)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x=> x.RefreshToken == token);

            if (result is not null)
            {
                result.RefreshToken = "";
                _dbContext.Users.Update(result);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
