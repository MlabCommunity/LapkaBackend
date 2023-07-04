using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using LapkaBackend.API.Requests;
using LapkaBackend.Infrastructure.Database.Data;
using LapkaBackend.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LapkaBackend.API.Controllers;


[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("/[controller]/loginWeb")]
    public ActionResult LoginWeb()
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Route("/[controller]/loginMobile")]
    public ActionResult LoginMobile([FromBody] LoginRequest user)
    {
        try
        {
            using UserContext context = new UserContext();
            var errors = new Errors();
            var userDb = context.Users.FirstOrDefault(x => x.Email == user.email && x.Password == user.password);
            if (userDb == null)
            {
                errors.AddError("invalid_credentials", "Invalid credentials");
                return BadRequest(errors.GetList());
            }
            
            var refreshToken = userDb.RefreshToken;
            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
            if(jwtSecurityToken.ValidTo <= DateTime.UtcNow)
            {
                userDb.RefreshToken = GenerateToken(DateTime.UtcNow.AddDays(3), "refresh_token");                
            }
            
            userDb.AccessToken = GenerateToken(DateTime.UtcNow.AddMinutes(5), "access_token");
            List<string> tokens = new List<string>();
            tokens.Add(userDb.AccessToken);
            tokens.Add(userDb.RefreshToken);
            return Ok(tokens);
        }
        catch (DbException)
        {
            return StatusCode(500);
        }
    }

    
    [HttpPost]
    [Route("/[controller]/userRegister")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult Register([FromBody] UserRegistrationRequest newUser)
    {
        try
        {
            using UserContext context = new UserContext();
            var errors = new Errors();
            if (ValidateData(newUser))
            {
                if (context.Users.Any(x => x.Email == newUser.email))
                    errors.AddError("user_exists", "User with this email already exists");
                if (newUser.password != newUser.passwordConfirm)
                    errors.AddError("invalid_password", "Passwords must be equal");
                context.Users.Add(new User
                {
                    FirstName = newUser.firstName,
                    LastName = newUser.lastName,
                    Email = newUser.email,
                    Password = newUser.password,
                    RefreshToken = GenerateToken(DateTime.UtcNow.AddDays(3), "refresh_token")
                });
                context.SaveChanges();
            }
            return errors.GetList().Count == 0 ? StatusCode(204): BadRequest(errors.GetList());

        }
        catch (DbException)
        {
            return StatusCode(500);
        }
    }
    
    
    private static string GenerateToken(DateTime expDate, string type)
    {
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YhlyL1kqamyhR1Q4FBHrIjOOyd6rtajB" ?? string.Empty));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("token_type", type)
        };
        var token = new JwtSecurityToken("issuer",
            "audience",
            claims,
            expires: expDate,
            signingCredentials: credentials);
        
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return generatedToken;
    }

    private static bool ValidateData(UserRegistrationRequest newUser)
    {
        var valid = true;
        var errors = new Errors();
        try
        {
            var email = new MailAddress(newUser.email);
        } 
        catch (FormatException) { 
            errors.AddError("invalid_email_address", 
                "'Email Address' is not in the correct format."); 
            valid = false;
        }
        if (!Regex.IsMatch(newUser.password, @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$" ))
        {
            errors.AddError("invalid_password", 
                "'Password' must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one special character.");
            valid = false;
        }
        return valid;
    }

}


// I think it should be in application layer
class Errors
{
    private static Errors? _instance;
    List<Error> _errors = new List<Error>();
    
    public void AddError(string code, string message)
    {
        _errors.Add(new Error { code = code, message = message });
    }

    public List<Error> GetList()
    {
        return _errors;
    }

}

class Error
{
    public string code { get; set; }
    public string message { get; set; }
}