using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;

namespace LapkaBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;

        public AuthService(IDataContext dbContext, IConfiguration configuration, 
            IEmailService emailService, IHttpContextAccessor contextAccessor)
        {
            // _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
            _contextAccessor = contextAccessor;
        }

        public async Task RegisterUser(UserRegistrationRequest request)
        {

            if (_dbContext.Users.Any(x => x.Email == request.EmailAddress))
            {
                throw new BadRequestException("invalid_email", "User with this email already exists");
            }
            
            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.EmailAddress,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                VerificationToken = CreateRandomToken(),
                RefreshToken = CreateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                Role = _dbContext.Roles.First(r => r.RoleName == Roles.User.ToString())
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            await SendEmailToConfirmEmail(newUser.Email, newUser.VerificationToken);
        }

        private async Task SendEmailToConfirmEmail(string emailAddress, string token)
        {
            var myUrl = new Uri(_contextAccessor.HttpContext!.Request.GetDisplayUrl());

            var baseUrl = myUrl.Scheme + Uri.SchemeDelimiter + myUrl.Authority;          

            var endpoint = $"/Auth/confirmEmail/{token}";

            var link = $"{baseUrl}{endpoint}";

            var mailRequest = new MailRequest()
            {
                ToEmail = emailAddress,
                Subject = "email confirmation",
                Template = Templates.Welcome,
                RedirectUrl = link
            };

            await _emailService.SendEmail(mailRequest);
        }

        public async Task<LoginResultDto> LoginUser(LoginRequest request)
        {
            var result = await _dbContext.Users
                .Include(u => u.Role)
                .Where(x => x.SoftDeleteAt == null)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new BadRequestException("invalid_email", "User doesn't exists");
            }

            if (result.VerifiedAt == null)
            {
                throw new ForbiddenException("not_verified", "Not verified");
            }
            
            if (!BCrypt.Net.BCrypt.Verify(request.Password, result.Password))
            {
                throw new BadRequestException("invalid_password", "Wrong password");
            }

            string refreshToken;
            
            if (IsTokenValid(result.RefreshToken))
            {
                refreshToken = result.RefreshToken;
            }
            else
            {
                refreshToken = CreateRefreshToken();
                result.RefreshToken = refreshToken;
                _dbContext.Users.Update(result);
                await _dbContext.SaveChangesAsync();
            }

            await SavingDataInCookies(result.Role.RoleName);
            
            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResultDto> LoginShelter(LoginRequest request)
        {
            var result = await _dbContext.Users
                .Where(x => x.SoftDeleteAt == null)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new BadRequestException("invalid_email", "User not found");
            }
            if (result.Role.RoleName != Roles.Shelter.ToString() && result.Role.RoleName != Roles.Worker.ToString())
            {
                throw new BadRequestException("invalid_role", "You are not Shelter!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, result.Password))
            {
                throw new BadRequestException("invalid_password", "Wrong password");
            }

            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : result.RefreshToken = CreateRefreshToken()
            };
        }

        public async Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request)
        {
            var jwtAccessToken = new JwtSecurityToken(request.AccessToken);

            if (jwtAccessToken == null)
            {
                throw new BadRequestException("invalid_token", "Invalid token");
            }

            if (!IsTokenValid(request.RefreshToken))
            {
                throw new BadRequestException("invalid_token", "Invalid token");
            }
            
            var emailClaim = jwtAccessToken.Claims.ToList().First(x => x.Type.Equals(ClaimTypes.Email));
            
            var user = await _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(c =>
                c.Email == emailClaim.Value);

            if (user == null)
            {
                throw new BadRequestException("invalid_email", "User doesn't exists");
            }
            
            var claims = new List<Claim>()
            {
                new("userId", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new UseRefreshTokenResultDto { AccessToken = jwt };
        }

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new("userId", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public string CreateRefreshToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public bool IsTokenValid(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = key
                }, out _);
            }
            catch (SecurityTokenValidationException e)
            {
                return false;
            }
            catch (Exception e)
            {
                _logger.Error(e, "token_error");
                return false;
            }
            return true;
        }

        public async Task RevokeToken(TokenRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);

            if (result is null)
            {
                throw new BadRequestException("invalid_token", "Refresh Token is invalid");
            }
            result.RefreshToken = "";
            _dbContext.Users.Update(result);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RegisterShelter(ShelterWithUserRegistrationRequest request)
        {
            if (_dbContext.Users.Any(x => x.Email == request.UserRequest.EmailAddress))
            {
                throw new BadRequestException("invalid_email", "Email already in use");
            }

            var roleUser = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == Roles.Shelter.ToString());

            var newShelter = new Shelter
            {
                OrganizationName = request.ShelterRequest.OrganizationName,
                Longitude = request.ShelterRequest.Longitude,
                Latitude = request.ShelterRequest.Latitude,
                City = request.ShelterRequest.City,
                Street = request.ShelterRequest.Street,
                ZipCode = request.ShelterRequest.ZipCode,
                Nip = request.ShelterRequest.Nip,
                Krs = request.ShelterRequest.Krs,
                PhoneNumber = request.ShelterRequest.PhoneNumber,
            };

            await _dbContext.Shelters.AddAsync(newShelter);

            var newUser = new User()
            {
                FirstName = request.UserRequest.FirstName,
                LastName = request.UserRequest.LastName,
                Email = request.UserRequest.EmailAddress,
                Password = BCrypt.Net.BCrypt.HashPassword(request.UserRequest.Password),
                RefreshToken = CreateRefreshToken(),
                VerificationToken = CreateRandomToken(),
                CreatedAt = DateTime.UtcNow,
                Role = roleUser!,
                ShelterId = newShelter.Id
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            await SendEmailToConfirmEmail(newUser.Email, newUser.VerificationToken);
        }

        public async Task ResetPassword(UserEmailRequest request)
        {
            var result = await _dbContext.Users
                .Where(x => x.SoftDeleteAt == null)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result is null)
            {
                throw new BadRequestException("invalid_mail", "User with that email does not exists");
            }
            
            var myUrl = new Uri(_contextAccessor.HttpContext!.Request.GetDisplayUrl());
            var baseUrl = myUrl.Scheme + Uri.SchemeDelimiter + myUrl.Authority;  
            var endpoint = $"/Auth/setPassword/{CreateSetNewPasswordToken(request.Email)}";

            var link = $"{baseUrl}{endpoint}";

            MailRequest mailRequest = new()
            {
                ToEmail = request.Email,
                Subject = "Reset password",
                Template = Templates.PasswordChange,
                RedirectUrl = link
            };

            await _emailService.SendEmail(mailRequest);
        }
        public async Task SetNewPassword(ResetPasswordRequest resetPasswordRequest, string token)
        {
            if (!IsTokenValid(token))
            {
                throw new BadRequestException("invalid_token", "Token is invalid");
            }

            if (resetPasswordRequest.Password != resetPasswordRequest.ConfirmPassword)
            {
                throw new BadRequestException("invalid_password", "Passwords aren't matching");
            }

            var userToken = new JwtSecurityToken(token);
            var userEmail = userToken.Claims.ToList().
                First(x => x.Type.Equals(ClaimTypes.Email));

            var user = await _dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == userEmail.Value);

            if (user is null)
            {
                throw new BadRequestException("invalid_email", "User doesn't exists");
            }
            
            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordRequest.Password);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        private string CreateSetNewPasswordToken(string emailAddress)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(x => x.Email == emailAddress) ?? throw new BadRequestException("invalid_email","There are no such email!");

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task ConfirmEmail(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user is null)
            {
                throw new BadRequestException("invalid_token", "Invalid Token");
            }

            user.VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            user.VerifiedAt = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SavingDataInCookies(string data)
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.Role, data)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1),
            };
            
            _contextAccessor.HttpContext!.Response.Cookies.Append("token", jwt, options);
        }
    }
}
