using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Application.Enums;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
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

        public async Task<GetUsersByRoleQueryResult> ListOfUsersWithTheSpecifiedRole(Roles role)
        {
            var roleName = role.ToString();

            if (roleName == Roles.SuperAdmin.ToString() || roleName == Roles.Undefined.ToString() || roleName == Roles.User.ToString())
            {
                throw new BadRequestException("invalid_role","Cannot choose SuperAdmin, Undefined, User");
            }

            var users = await _dbContext.Users
            .Where(u => u.Role!.RoleName == roleName)
            .ToListAsync();

            var result = new GetUsersByRoleQueryResult
            {
                Users = _mapper.Map<List<UserDto>>(users)
            };


            return result;
        }

        public async Task AssignAdminRole(Guid userId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role!.RoleName != Roles.Shelter.ToString() && userResult.Role.RoleName != Roles.User.ToString())
            {
                int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.Admin.ToString()).Select(r => r.Id).FirstOrDefaultAsync();
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

            int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.Worker.ToString()).Select(r => r.Id).FirstOrDefaultAsync();
            userResult.Role.Id = searchedRoleId;
            await _dbContext.SaveChangesAsync();

        }
    }
}

