﻿using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
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
        
        public async Task RegisterUser(UserRegistrationRequest request)
        {
            
            if (_dbContext.Users.Any(x => x.Email == request.Email))
            {
                throw new AuthException("User already exists", 400);
            }

            var RoleUser = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "Worker");
            if (RoleUser == null)
            {
                RoleUser = new Role
                {
                    RoleName = "Worker"
                };

                await _dbContext.Roles.AddAsync(RoleUser);
                await _dbContext.SaveChangesAsync();
            }

            var newUser = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = DateTime.Now,
                Role = RoleUser
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LoginResultDto> LoginUser(LoginRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new AuthException("User not found", AuthException.StatusCodes.BadRequest);
            }

            if (result.Password != request.Password)
            {
                throw new AuthException("Wrong password", AuthException.StatusCodes.BadRequest);
            }
            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : GenerateRefreshToken()
            }; 
        }

        public async Task<LoginResultDto> LoginShelter (LoginRequest request)
        {
            var result = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (result == null)
            {
                throw new AuthException("User not found", AuthException.StatusCodes.BadRequest);
            }
            if (result.Role.RoleName != "Shelter" )
            {
                throw new AuthException("You are not Shelter !", AuthException.StatusCodes.BadRequest);
            }

            if (result.Password != request.Password)
            {
                throw new AuthException("Wrong password", AuthException.StatusCodes.BadRequest);
            }
            return new LoginResultDto
            {
                AccessToken = CreateAccessToken(result),
                RefreshToken = IsTokenValid(result.RefreshToken) ? result.RefreshToken : GenerateRefreshToken()
            };
        }

        public async Task<UseRefreshTokenResultDto> RefreshAccessToken(UseRefreshTokenRequest request) 
        {
            // TODO: (Najważniejsze) dodać metode refreshaccesstoken ma przyjmować tokeny a create usera 
            // TODO: Do przeanalizowania struktura

            var jwtAccesToken = new JwtSecurityToken(request.AccessToken);

            if (jwtAccesToken == null)
            {
                throw new AuthException("Błędny token", AuthException.StatusCodes.BadRequest);
            }
            
            var user = await _dbContext.Users.FirstAsync(c => c.Email == jwtAccesToken.Claims.First(x => x.Type == ClaimTypes.Email).Value);

            if(user == null)
            {
                throw new AuthException("Nie znaleziono użytkownika", AuthException.StatusCodes.BadRequest);
            }

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, "User")
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

            return new UseRefreshTokenResultDto { AccessToken = jwt};
        }

        public string CreateAccessToken(User user)
        { 

            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Role, "User")
                // TODO: Change User to user.Role 
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

        public async Task SaveRefreshToken(LoginRequest request, string newRefreshToken)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

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

        public async Task RevokeToken(TokenRequest request)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x=> x.RefreshToken == request.RefreshToken);

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
                throw new AuthException("Shelter already exists", 400);
            }
            var RoleUser = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "Shelter");
            if (RoleUser == null)
            {
                RoleUser = new Role
                {
                    RoleName = "Shelter"
                };

                await _dbContext.Roles.AddAsync(RoleUser);
                await _dbContext.SaveChangesAsync();

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
                    Role = RoleUser,
                    ShelterId = newShelter.Id
                };

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ResetPassword(string emailAddress)
        {
            string body = "Test Changing Password";
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("casimir28@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("emailAddress"));
            email.Subject = "Changing password";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email",587,MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("casimir28@ethereal.email", "GAmcfRXj4CgvZuDyVm");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
