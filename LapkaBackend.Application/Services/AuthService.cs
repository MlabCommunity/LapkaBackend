using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace LapkaBackend.Infrastructure.Services
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


        #region RegisterUser
        public async Task<User> RegisterUser(UserRegisterDto user)
        {

            if (user.Password != user.ConfirmPassword)
            {
                return null;
            }

            var newUser = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                CreatedAt = DateTime.Now
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser;
        }
        #endregion

        #region LoginUser
        public string LoginUser(UserLoginDto user)
        {
            var result = _dbContext.Users.FirstOrDefault(x => x.Email == user.Email);

            if (result == null)
            {
                return "Taki użytkownik nie istnieje";
            }

            if (result.Password != user.Password)
            {
                return "Hasła się nie zgadzają";
            }

            string token = CreateToken(result);

            return (token);
        }
        #endregion

        #region CreateToken
        public string CreateToken(User user) 
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "Admin") // TODO: zamiast Admin ma być zmienna Rola z Encji User 
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
        #endregion

        #region GenerateRefreshToken
        public TokenDto GenerateRefreshToken()
        {
            var refreshToken = new TokenDto
            {
                RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TokenExpire = DateTime.Now.AddDays(7),
                TokenCreated = DateTime.Now
            };

            return refreshToken;
        }
        #endregion

        #region SaveRefreshTokenInDb
        public async Task SaveRefreshToken(UserLoginDto user, TokenDto newRefreshToken)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

            result.RefreshToken = newRefreshToken.RefreshToken;
            result.TokenCreated = newRefreshToken.TokenCreated;
            result.TokenExpire = newRefreshToken.TokenExpire;

            _dbContext.Users.Update(result);

            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region IsAccesTokenValid
        public bool IsAccesTokenValid(string token)
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
        #endregion

        #region RevokeToken
        public async Task RevokeToken(string token)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x=> x.RefreshToken == token);

            if (result != null)
            {
                result.RefreshToken = "";
                // TODO: Dopytać czy można ustawić jakoś datetime na pusty
                _dbContext.Users.Update(result);
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}
