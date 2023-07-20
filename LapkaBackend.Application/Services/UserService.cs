using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using System.Security.Cryptography;
using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDataContext _context;
        private readonly IEmailService _emailService;

        public UserService(IDataContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var result = await _context.Users.FindAsync(id);

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            return result;
        }

        public async Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(UpdateUserDataRequest request, string id)
        {
            var result = await GetUserById(new Guid(id));

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            result.FirstName = request.FirstName;
            result.LastName = request.LastName;
            //TODO: odkomentować po dodaniu profilepicture
            //result.ProfilePicture = request.ProfilePicture;

            _context.Users.Update(result);

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task DeleteUser(string id)
        {
            var result = await GetUserById(new Guid(id));

            if (result is null)
            {
                throw new BadRequestException("invalid_user","User doesn't exists");
            }

            _context.Users.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindUserByRefreshToken(TokenRequest request)
        {
            var result = await _context.Users
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
            var result = await _context.Users
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
            var user = await GetUserById(new Guid(id));

            user.Password = request.NewPassword;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SetNewEmail(string id, UpdateUserEmailRequest request)
        {
            var user = await GetUserById(new Guid(id));

            user.Email = request.Email;
            user.VerifiedAt = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _emailService.SendEmail(new MailRequest
            {
                ToEmail = request.Email,
                Subject = "Zmiana emaila",
                Body = $"https://localhost:7214/User/ConfirmUpdatedEmail/{user.VerificationToken}"
            });
        }

        //TODO: Do podmiany typ zwracany User na GetCurrentUserDataQueryResult po dodaniu loginProvider'a i profilePicture
        public async Task<User> GetLoggedUser(string id)
        {
            var user = await GetUserById(new Guid(id));

            return user;
        }

        public async Task VerifyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            
            if(user is null)
            {
                throw new BadRequestException("invalid_token","Invalid Token");
            }

            user.VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            user.VerifiedAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
