using LapkaBackend.Application.Common;
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

        public async Task<User> FindUserByRefreshToken(string refreshToken)
        {
            var myUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (myUser == null) 
            {
                return null;
            }

            return myUser;
        }
    }
}
