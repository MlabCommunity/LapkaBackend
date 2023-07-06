using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<User> RegisterUser(Auth auth)
        {

            if (auth.Password != auth.ConfirmPassword)
            {
                return null;
            }

            var newUser = new User()
            {
                FirstName = auth.FirstName,
                LastName = auth.LastName,
                Email = auth.Email,
                Password = auth.Password,
                CreatedAt = DateTime.Now
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser;
        }
        #endregion

        #region LoginUser
        public string LoginUser(User user)
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
                new Claim(ClaimTypes.Role, "Admin") // zamiast Admin ma być zmienna Rola z Encji User 
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
        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expire = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }
        #endregion

        #region SaveRefreshTokenInDb
        public async Task SaveRefreshToken(User user, RefreshToken newRefreshToken)
        {
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpire = newRefreshToken.Expire;

            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
