using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.ApplicationDtos;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LapkaBackend.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("userRegister")]
        public async Task<ActionResult<User>> UserRegister(UserDto userDto)
        {
            return await (_authService.UserRegister(userDto));
        }



        [HttpPost("shelterRegister")] // Rejestracja schroniska wraz z danymi użytkownika 
        public async Task<ActionResult<Shelter>> ShelterRegister(RegistrationRequest RegistrationRequest)
        {
            //await _authService.UserRegister(userDto);
            //await _authService.ShelterRegister(shelterDto);
            //return ();
            var userResult = await _authService.UserRegister(RegistrationRequest.UserDto);
            var shelterResult = await _authService.ShelterRegister(RegistrationRequest.ShelterDto);

            if (userResult.Result is BadRequestResult || shelterResult.Result is BadRequestResult)
            {
                return BadRequest("fields filled in incorrectly");
            }
            else if(userResult.Result is OkObjectResult || shelterResult.Result is OkObjectResult)
            {
                

                return Ok(shelterResult.Value);
            }
            else
                return StatusCode(500, "Sorry, something went wrong");
        }



        /*
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        
        
       


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
        }

        
        

        

         */
    }
}
