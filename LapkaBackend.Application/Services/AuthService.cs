using LapkaBackend.Application.ApplicationDtos;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILapkaBackendDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration, ILapkaBackendDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }
            public async Task<ActionResult<User>> UserRegister(UserDto userDto)
        {
            if (!(string.IsNullOrWhiteSpace(userDto.firstName) && string.IsNullOrWhiteSpace(userDto.lastName) && string.IsNullOrWhiteSpace(userDto.emailAddress) && string.IsNullOrWhiteSpace(userDto.password) && string.IsNullOrWhiteSpace(userDto.confirmPassword)))
            {
                if (userDto.password == userDto.confirmPassword)
                {
                    CreatePasswordHash(userDto.password, out byte[] passwordHash, out byte[] passwordSalt);

                    DateTime dateTimeNow = DateTime.Now;
                    string createdAt = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");

                    var user = new User();
                    user.FirstName = userDto.firstName;
                    user.LastName = userDto.lastName;
                    user.Email = userDto.emailAddress;
                    user.CreatedAt = createdAt;
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();

                    return new OkObjectResult(user);
                }

                return new BadRequestObjectResult("Passwords don't match");
            }
            return new BadRequestObjectResult("Complete the required fields");
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        // Rejestracja schroniska
        public async Task<ActionResult<Shelter>> ShelterRegister(ShelterDto shelterDto)
        {
            if (!(string.IsNullOrWhiteSpace(shelterDto.City) && string.IsNullOrWhiteSpace(shelterDto.Krs) && string.IsNullOrWhiteSpace(shelterDto.Nip) && string.IsNullOrWhiteSpace(shelterDto.OrganizationName) && string.IsNullOrWhiteSpace(shelterDto.phoneNumber) && string.IsNullOrWhiteSpace(shelterDto.Street) && string.IsNullOrWhiteSpace(shelterDto.ZipCode)))
            {
                var shelter = new Shelter();
                shelter.OrganizationName = shelterDto.OrganizationName;
                shelter.Longtitude = shelterDto.Longtitude;
                shelter.Latitude = shelterDto.Latitude;
                shelter.City = shelterDto.City;
                shelter.Street = shelterDto.Street;
                shelter.ZipCode = shelterDto.ZipCode;
                shelter.Nip = shelterDto.Nip;
                shelter.Krs = shelterDto.Krs;
                shelter.phoneNumber = shelterDto.phoneNumber;

                _dbContext.Shelters.Add(shelter);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(shelter);
            }
            return new NoContentResult();
        }





    }
}
