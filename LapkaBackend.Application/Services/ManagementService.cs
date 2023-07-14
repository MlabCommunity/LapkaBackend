using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _configuration;

        public ManagementService(IDataContext dbContext, IConfiguration configuration)
        {

            _dbContext = dbContext;
            _configuration = configuration;
        }


        
        public async Task AssignAdminRole(Guid userId)
        {
            // trzeba sprawdzić czy user jest zalogowany jako superadmin

            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult.Role.RoleName != "Shelter"  && userResult.Role.RoleName != "User")
            {
                int adminRoleId = await _dbContext.Roles.Where(r => r.RoleName == "Admin").Select(r => r.Id).FirstOrDefaultAsync();

                if (adminRoleId == 0)
                {
                    Role RoleAdmin = new Role
                    {
                        RoleName = "Admin"
                    };
                    await _dbContext.Roles.AddAsync(RoleAdmin);
                    await _dbContext.SaveChangesAsync();

                    userResult.RoleId = RoleAdmin.Id;
                    await _dbContext.SaveChangesAsync();
                }
                else 
                {
                    userResult.RoleId = adminRoleId;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<User>> ListOfUsersWithTheSpecifiedRole(string roleName)
        {
            var users = await _dbContext.Users
            .Where(u => u.Role.RoleName == roleName)
            .ToListAsync();
            
            return users;
        }

        public async Task AssignRemoveAdminRole(Guid userId, string newStringRole)
        {
            // trzeba sprawdzić czy user jest zalogowany jako superadmin

            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);
            if (userResult.Role.RoleName != "Shelter" && userResult.Role.RoleName != "User")
            {
                int srearchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == newStringRole).Select(r => r.Id).FirstOrDefaultAsync();

                if (srearchedRoleId == 0)
                {
                    Role RoleNew = new Role
                    {
                        RoleName = newStringRole
                    };
                    await _dbContext.Roles.AddAsync(RoleNew);
                    await _dbContext.SaveChangesAsync();

                    userResult.RoleId = RoleNew.Id;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    userResult.RoleId = srearchedRoleId;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }


    }
}
