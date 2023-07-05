using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using LapkaBackend.Application.Dto;
using System.Runtime.InteropServices;
using LapkaBackend.Domain.Models;
using Microsoft.Extensions.Configuration;
using LapkaBackend.Domain.Entities;

namespace LapkaBackend.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LapkaBackendDBContext _dbContext;
        public AuthController(LapkaBackendDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

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


        /*
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user); 
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
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

        

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }

        } */
    }
}
