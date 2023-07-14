using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDataContext _context;

        public UserService(IDataContext context)
        {
            _context = context;
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
                throw new AuthException("User doesn't exists", AuthException.StatusCodes.BadRequest);
            }

            return result;
        }

        public async Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(User user, Guid id)
        {
            //TODO: Zwracać DTO GetUserDataBuIdQueryResult
            var result = await GetUserById(id);

            if (result is null)
            {
                throw new AuthException("User doesn't exists", AuthException.StatusCodes.BadRequest);
            }

            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.Email = user.Email;

            _context.Users.Update(result);

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task DeleteUser(Guid id)
        {
            var result = await GetUserById(id);

            if (result is null)
            {
                throw new AuthException("User doesn't exists", AuthException.StatusCodes.BadRequest);
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
                throw new AuthException("User doesn't exists", AuthException.StatusCodes.BadRequest);
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
                throw new AuthException("User doesn't exists", AuthException.StatusCodes.BadRequest);
            }

            return result;
        }
    }
}
