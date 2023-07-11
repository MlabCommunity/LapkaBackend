using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            //TODO: Wyjątek z możliwym zwrotem nulla, dodać new exception

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
            var result = await GetUserById(id);

            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.Email = user.Email;

            _context.Users.Update(result);

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<User> DeleteUser(Guid id)
        {
            var result = await GetUserById(id);

            _context.Users.Remove(result);
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<User> FindUserByRefreshToken(TokensDto token)
        {
            var myUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.RefreshToken == token.RefreshToken);

            if (myUser == null) 
            {
                return null; // TODO: Tu powinien być custom wyjątek
            }

            return myUser;
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var myUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (myUser == null)
            {
                return null; // TODO: Tu powinien być custom wyjątek
            }

            return myUser;
        }
    }
}
