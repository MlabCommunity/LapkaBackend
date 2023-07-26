using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using System.Security.Cryptography;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDataContext _dbContext;
        private readonly IEmailService _emailService;

        public UserService(IDataContext context, IEmailService emailService)
        {
            _dbContext = context;
            _emailService = emailService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();
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

        public async Task<User> AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(UpdateUserDataRequest request, string id)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            result.FirstName = request.FirstName;
            result.LastName = request.LastName;
            //TODO: odkomentować po dodaniu profilepicture
            //result.ProfilePicture = request.ProfilePicture;

            _dbContext.Users.Update(result);

            await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task DeleteUser(string id)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            _dbContext.Users.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> FindUserByRefreshToken(TokenRequest request)
        {
            var result = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
                
            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            return result;
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var result = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            return result;
        }

        public async Task SetNewPassword(string id, UserPasswordRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));

            user!.Password = request.NewPassword;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetNewEmail(string id, UpdateUserEmailRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));

            user!.Email = request.Email;
            user.VerifiedAt = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            string baseUrl = "https://localhost:7214";
            string token = user.VerificationToken!;
            string endpoint = $"/User/ConfirmUpdatedEmail/{token}";

            string link = $"{baseUrl}{endpoint}";

            await _emailService.SendEmail(new MailRequest
            {
                ToEmail = request.Email,
                Subject = "Zmiana emaila",
                Template = Templates.ConfirmEmailChange,
                RedirectUrl = link
            });
        }

        public async Task<GetCurrentUserDataQueryResult> GetLoggedUser(string id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Guid(id));

            return new GetCurrentUserDataQueryResult
            {
                Id = user!.Id,
                Username = "xd",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = (Roles)user.Role!.Id
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
            user.VerifiedAt = DateTime.Now;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
