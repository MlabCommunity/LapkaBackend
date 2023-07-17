using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LapkaBackend.Application.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ManagementService(IDataContext dbContext, IConfiguration configuration, IMapper mapper)
        {

            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> ListOfUsersWithTheSpecifiedRole(string roleName)
        {
            if (roleName == "SuperAdmin" || roleName == "Undefined" || roleName == "User")
            {
                throw new AuthException("Wrong role name!");
            }

            var users = await _dbContext.Users
            .Where(u => u.Role.RoleName == roleName)
            .ToListAsync();

            var usersDtos = _mapper.Map<List<UserDto>>(users);

            return usersDtos;
        }

        public async Task AssignRemoveAdminRole(Guid userId, string newStringRole)
        {
            // trzeba sprawdzić czy user jest zalogowany jako superadmin

            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new AuthException("User not found!", AuthException.StatusCodes.BadRequest);
            }

            if (newStringRole == "Worker" && userResult.Role.RoleName != "Admin")
            {
                throw new AuthException("User is not admin!", AuthException.StatusCodes.Forbidden);
            }

            if (userResult.Role.RoleName != "Shelter" && userResult.Role.RoleName != "User")
            {
                int srearchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == newStringRole).Select(r => r.Id).FirstOrDefaultAsync();

                if (srearchedRoleId == 0)
                {
                    //TODO: Do wywalenia przy dodaniu roli
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
