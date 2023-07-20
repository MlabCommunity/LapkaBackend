using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Enums;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Services
{
    public class ManagementService : IManagementService
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public ManagementService(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> ListOfUsersWithTheSpecifiedRole(string roleName)
        {
            if (roleName == RoleName.SuperAdmin.ToString() || roleName == RoleName.Undefined.ToString() || roleName == RoleName.User.ToString())
            {
                throw new BadRequestException("invalid_role","Role not found!");
            }

            var users = await _dbContext.Users
            .Where(u => u.Role!.RoleName == roleName)
            .ToListAsync();

            var usersDtos = _mapper.Map<List<UserDto>>(users);

            return usersDtos;
        }

        public async Task AssignAdminRole(Guid userId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role!.RoleName != RoleName.Shelter.ToString() && userResult.Role.RoleName != RoleName.User.ToString())
            {
                int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == RoleName.Admin.ToString()).Select(r => r.Id).FirstOrDefaultAsync();
                userResult.RoleId = searchedRoleId;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAdminRole(Guid userId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user","User not found!");
            }

            if (userResult.Role!.RoleName != "Admin")
            {
                throw new ForbiddenExcpetion("invalid_user","User is not an admin!");
            }

            int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == RoleName.Worker.ToString()).Select(r => r.Id).FirstOrDefaultAsync();
            userResult.Role.Id = searchedRoleId;
            await _dbContext.SaveChangesAsync();

        }
    }
}

