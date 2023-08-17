﻿using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using System.Security.Cryptography;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace LapkaBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDataContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IBlobService _blobService;
        private readonly IHttpContextAccessor _contextAccessor;
        

        public UserService(IDataContext context, IEmailService emailService,
            IBlobService blobService,
            IHttpContextAccessor contextAccessor)
        {
            _dbContext = context;
            _emailService = emailService;
            _blobService = blobService;
            _contextAccessor = contextAccessor;
        }
        public async Task<GetUserDataByIdQueryResult> GetUserById(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            return new GetUserDataByIdQueryResult
            {
                Id = user.Id,
                Username = "xd",
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task UpdateUser(UpdateUserDataRequest request, Guid id)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            
            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            if (request.ProfilePicture is not null && request.ProfilePicture != result.ProfilePicture)
            {
                if (result.ProfilePicture != string.Empty && result.ProfilePicture is not null)
                {
                    await _blobService.DeleteFileAsync(new Guid(result.ProfilePicture));     
                }
                result.ProfilePicture = request.ProfilePicture;
                var file = _dbContext.Blobs.First(x => x.Id == new Guid(request.ProfilePicture));
                file.ParentEntityId = result.Id;

            }

            result.FirstName = request.FirstName;
            result.LastName = request.LastName;
            _dbContext.Users.Update(result);

            await _dbContext.SaveChangesAsync();

        }

        public async Task DeleteUser(Guid id)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            result.SoftDeleteAt = DateTime.UtcNow.AddMonths(1);
            _dbContext.Users.Update(result);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetNewPassword(Guid id, UserPasswordRequest request)
        {
            var user = await _dbContext.Users.
                FirstOrDefaultAsync(x => x.Id == id);

            if (user is null)
            {
                throw new BadRequestException("invalid_request", "User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                throw new BadRequestException("invalid_password", "Invalid current password");
            }
            
            user!.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetNewEmail(Guid id, UpdateUserEmailRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            user!.Email = request.Email;
            user.VerifiedAt = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            var myUrl = new Uri(_contextAccessor.HttpContext!.Request.GetDisplayUrl());
            var baseUrl = myUrl.Scheme + Uri.SchemeDelimiter + myUrl.Authority;
            var endpoint = $"/User/ConfirmUpdatedEmail/{user.VerificationToken}";

            var link = $"{baseUrl}{endpoint}";

            await _emailService.SendEmail(new MailRequest
            {
                ToEmail = request.Email,
                Subject = "Zmiana emaila",
                Template = Templates.ConfirmEmailChange,
                RedirectUrl = link
            });
        }

        public async Task<GetCurrentUserDataQueryResult> GetLoggedUser(Guid id)
        {
            var user = await _dbContext.Users.Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            return new GetCurrentUserDataQueryResult
            {
                Id = user!.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                ProfilePicture = user.ProfilePicture,
                Role = (Roles)user.Role.Id,
                LoginProvider = user.LoginProvider
            };
        }

        public async Task VerifyEmail(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            
            if(user is null)
            {
                throw new BadRequestException("invalid_token","Invalid Token");
            }

            user.VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            user.VerifiedAt = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task DeleteProfilePicture(Guid id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.ToString().Equals(user.ProfilePicture));

            if (file is null)
            {
                throw new BadRequestException("invalid_file","User doesn't have profile picture");
            }
            
            user.ProfilePicture = string.Empty;
            _dbContext.Users.Update(user);
            await _blobService.DeleteFileAsync(file.Id);
            await _dbContext.SaveChangesAsync();
        }
    }
}
