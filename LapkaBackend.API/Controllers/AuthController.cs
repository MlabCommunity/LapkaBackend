using Microsoft.AspNetCore.Mvc;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.ApplicationDtos;


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
            return await _authService.UserRegister(userDto);
        }

        [HttpPost("shelterRegister")] // Rejestracja schroniska wraz z danymi użytkownika
        public async Task<ActionResult<Shelter>> ShelterRegister(RegistrationRequest RegistrationRequest)
        {
            var userResult = await _authService.UserRegister(RegistrationRequest.UserDto);
            var shelterResult = await _authService.ShelterRegister(RegistrationRequest.ShelterDto);

            if (userResult.Result is BadRequestObjectResult || shelterResult.Result is BadRequestObjectResult)
            {
                return BadRequest("fields filled in incorrectly");
            }
            else if(userResult.Result is OkObjectResult || shelterResult.Result is OkObjectResult)
            {
                return Ok();
            }
            else
                return StatusCode(500, "Sorry, something went wrong");
        }

        /* 
        Do zrobienia:
        Potwierdzenie maila podanego przy rejestracji
        logowanie pracownika i schroniska - zwracanie tokenów
        */

        [HttpPost("loginMobile")]
        public async Task<ActionResult<TokenResponse>> LoginMobile(LoginUserDto loginUserDto)
        {
            return await _authService.Login(loginUserDto);
        }
        
    }
}
