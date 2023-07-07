using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Requests;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IDataContext _context;

        public UserService(IDataContext context)
        {
            _context = context;
        }

        #region GetAllUsers
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        #endregion

        #region GetUserById
        public async Task<User> GetUserById(int id)
        {
            var result = await _context.Users.FindAsync(id);

            //TODO: Wyjątek z możliwym zwrotem nulla, dodać new exception

            return result;
        }
        #endregion

        #region AddUser
        public async Task<List<User>> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await _context.Users.ToListAsync();
        }
        #endregion

        #region UpdateUser
        public async Task<List<User>> UpdateUser(User user, int id)
        {
            var result = await GetUserById(id);

            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.Email = user.Email;

            _context.Users.Update(result);

            await _context.SaveChangesAsync();

            return await _context.Users.ToListAsync();
        }
        #endregion

        #region DeleteUser
        public async Task<List<User>> DeleteUser(int id)
        {
            var result = await GetUserById(id);

            _context.Users.Remove(result);
            await _context.SaveChangesAsync();

            return await _context.Users.ToListAsync();
        }
        #endregion

        #region FindUserByRefreshToken
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
        #endregion

        #region FindUserByRefreshToken
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
        #endregion
    }
}
