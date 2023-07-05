using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using LapkaBackend.Application.Dto;
using LapkaBackend.Domain.Models;
using Microsoft.Extensions.Configuration;
using LapkaBackend.Domain.Common;

namespace LapkaBackend.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly ILapkaBackendDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration, ILapkaBackendDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            DateTime dateTimeNow = DateTime.Now;
            string createdAt = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");

            var user = new User();
            user.firstName = userDto.firstName;
            user.lastName = userDto.lastName;
            user.email = userDto.email;
            user.createdAt = createdAt;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(user);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        
        
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string requestEmail, string requestPassword)
        {
            User? user =  _dbContext.Users.FirstOrDefault(r => r.email == requestEmail);

            if (user !=null)
            {
                if (!VerifyPasswordHash(requestPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password.");
                }
                else
                {
                    string token = CreateToken(user);
                    return Ok(token);
                }
                    
            }
            

             
            return BadRequest("Wrong email.");
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }

        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.firstName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)); // tu błąd System.ArgumentOutOfRangeException: IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'http://www.w3.org/2001/04/xmldsig-more#hmac-sha512', the key size must be greater than: '512' bits, key has '136' bits. (Parameter 'keyBytes')

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        /*
        

        

         */
    }
}
