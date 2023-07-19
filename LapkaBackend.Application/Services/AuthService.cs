using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Helpter;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace LapkaBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IDataContext dbContext, IConfiguration configuration, IEmailService emailService)
        {

            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task RegisterUser(UserRegistrationRequest request)
        {

            if (_dbContext.Users.Any(x => x.Email == request.Email))
            {
                throw new BadRequestException("invalid_email", "User with this email already exists");
            }

            var role = _dbContext.Roles.First(r => r.RoleName.ToUpper() == "USER");

            var newUser = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.Now,
                Role = role
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LoginResultDto> LoginUser(LoginRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new BadRequestException("invalid_email", "User doesn't exists");
            }
            /*
            if (result.VerifiedAt == null)
            {
                throw new ForbiddenExcpetion("not_verified", "Not verified");
            }   */

            if (result.Password != request.Password)
            {
                throw new BadRequestException("invalid_password", "Wrong password");
            }

            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : GenerateRefreshToken()
            };
        }

        public async Task<LoginResultDto> LoginShelter(LoginRequest request)
        {
            var result = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new BadRequestException("invalid_mail", "User not found");
            }
            // if (result.Role!.RoleName.ToUpper() != "SHELTER" && result.Role.RoleName.ToUpper() != "WORKER" )
            // {
            //     throw new BadRequestException("","You are not Shelter!");
            // }
            // TODO replace with authorize

            if (result.Password != request.Password)
            {
                throw new BadRequestException("invalid_password", "Wrong password");
            }

            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : GenerateRefreshToken()
            };
        }

        public async Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request)
        {
            var jwtAccesToken = new JwtSecurityToken(request.AccessToken);

            if (jwtAccesToken == null)
            {
                throw new BadRequestException("invalid_token", "Invalid token");
            }

            var user = await _dbContext.Users.FirstAsync(c =>
                c.Email == jwtAccesToken.Claims.First(x => x.Type == ClaimTypes.Email).Value);

            if (user == null)
            {
                throw new BadRequestException("", "User doesn't exists");
            }

            var role = await _dbContext.Roles.FirstAsync(x => x.Id == user.RoleId);

            List<Claim> claims = new List<Claim>()
            {
                new("userId", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role.RoleName)
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

            return new UseRefreshTokenResultDto { AccessToken = jwt };
        }

        public string CreateAccessToken(User user)
        {

            var role = _dbContext.Roles.First(x => x.Id == user.RoleId);

            List<Claim> claims = new List<Claim>()
            {
                new("userId", user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role.RoleName)
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

        public string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public async Task SaveRefreshToken(LoginRequest request, string newRefreshToken)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result is null)
            {
                throw new BadRequestException("invalid_email", "User doesn't exists");
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

        public async Task RevokeToken(TokenRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);

            if (result is not null)
            {
                result.RefreshToken = "";
                _dbContext.Users.Update(result);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RegisterShelter(ShelterWithUserRegistrationRequest request)
        {
            if (_dbContext.Users.Any(x => x.Email == request.UserRequest.Email))
            {
                throw new BadRequestException("invalid_email", "Shelter already exists");
            }

            var roleUser = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName.ToUpper() == "SHELTER");
            if (roleUser == null)
            {
                roleUser = new Role
                {
                    RoleName = "Shelter"
                };
                var newShelter = new Shelter()
                {
                    OrganizationName = request.ShelterRequest.OrganizationName,
                    Longtitude = request.ShelterRequest.Longitude,
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
                    Email = request.UserRequest.Email,
                    Password = request.UserRequest.Password,
                    RefreshToken = GenerateRefreshToken(),
                    CreatedAt = DateTime.Now,
                    Role = roleUser,
                    ShelterId = newShelter.Id
                };

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ResetPassword(string emailAddress)
        {
            string baseUrl = "https://localhost:7214"; //""
            string token = CreateSetNewPasswordToken(emailAddress);
            string endpoint = $"/Auth/setPassword/{token}";

            string link = $"{baseUrl}{endpoint}";

            Mailrequest mailrequest = new Mailrequest()
            {
                ToEmail = emailAddress,
                Subject = "Reset password",
                Body = "That is your link for changing password:" + link

            };

            await _emailService.SendEmail(mailrequest);
        }

        public async Task SetNewPassword(ResetPasswordRequest resetPasswordRequest, string token)
        {
            string? email = VerifyToken(token);
            if (email == null)
            {
                throw new BadRequestException("invalid_token", "Token is invalid");
            }
            if (resetPasswordRequest.Password != resetPasswordRequest.ConfirmPassword)
            {
                throw new BadRequestException("invalid_password", "Passwords aren't matching");
            }

            User user =  _dbContext.Users.Include(u => u.Role).FirstOrDefault(x => x.Email == email)!;

            user.Password = resetPasswordRequest.Password;

            await _dbContext.SaveChangesAsync();
        }

        public string CreateSetNewPasswordToken(string emailAddress)
        {
            User user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(x => x.Email == emailAddress) ?? throw new BadRequestException("invalid_email","There are no such email!");

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.LastName) 
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

        private string? VerifyToken(string token)
        {
            if (!IsTokenValid(token))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                User? user = _dbContext.Users.Include(u => u.Role).FirstOrDefault(x => x.Email == email);
                if (user == null)
                {
                    return null;
                }

                return email;
            }
            catch
            {
                return null;
            }
        }
    }

}
