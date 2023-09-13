using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Dtos; 
using LapkaBackend.Application.Dtos.Result;
using LapkaBackend.Domain.Enums;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

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

            if (role is Roles.SuperAdmin or Roles.Undefined or Roles.User)
            {
                throw new BadRequestException("invalid_role","Cannot choose SuperAdmin, Undefined, User");
            }
            
            var users = await _dbContext.Users
            .Where(u => u.Role.RoleName == role.ToString() && u.SoftDeleteAt == null)
            .ToListAsync();

            var result = new GetUsersByRoleQueryResult
            {
                Users = _mapper.Map<List<UserDto>>(users)
            };

            return result;
        }

        public async Task AssignAdminRole(Guid userId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role)
                .Where(x => x.SoftDeleteAt == null)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role.RoleName != Roles.Shelter.ToString() && 
                userResult.Role.RoleName != Roles.User.ToString())
            {
                var searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.Admin.ToString())
                    .Select(r => r.Id).FirstOrDefaultAsync();
                userResult.RoleId = searchedRoleId;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveAdminRole(Guid userId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role)
                .Where(x =>  x.SoftDeleteAt == null)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user","User not found!");
            }

            if (userResult.Role.RoleName != Roles.Admin.ToString())
            {
                throw new ForbiddenException("invalid_user","User is not an admin!");
            }

            var searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.User.ToString())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
            userResult.Role.Id = searchedRoleId;
            await _dbContext.SaveChangesAsync();

        }

        public async Task AddWorkerByAdmin(string userIdstring)
        {
            Guid userId = new Guid(userIdstring);
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role.RoleName != "User")
            {
                throw new BadRequestException("invalid_user", "You can not assing worker role to user with other role than user");
            }

            int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.Worker.ToString()).Select(r => r.Id).FirstOrDefaultAsync();
            userResult.RoleId = searchedRoleId;
            _dbContext.Users.Update(userResult);
            await _dbContext.SaveChangesAsync();

        }

        public async Task RemoveWorkerByAdmin(string userIdstring)
        {
            Guid userId = new Guid(userIdstring);
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == userId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role.RoleName != "Worker")
            {
                throw new BadRequestException("invalid_user", "You can not remove worker role to user with other role than worker");
            }

            int searchedRoleId = await _dbContext.Roles.Where(r => r.RoleName == Roles.User.GetDisplayName()).Select(r => r.Id).FirstOrDefaultAsync();
            userResult.RoleId = searchedRoleId;
            _dbContext.Users.Update(userResult);
            await _dbContext.SaveChangesAsync();

        }

        public async Task RemoveWorkerByShelter(string email, Guid shelterId)
        {
            var userResult = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Email == email && x.ShelterId==shelterId);

            if (userResult is null)
            {
                throw new BadRequestException("invalid_user", "User not found!");
            }

            if (userResult.Role.RoleName != "Worker")
            {
                throw new BadRequestException("invalid_user", "You can not remove anyone other than worker");
            }

            _dbContext.Users.Remove(userResult);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GetUsersByRoleQueryResult> ListOfWorkersInShelter(Guid shelterId)
        {
            var users = await _dbContext.Users
            .Where(u => u.Role.RoleName == Roles.Worker.ToString() && u.ShelterId == shelterId && u.SoftDeleteAt == null)
            .ToListAsync();

            var result = new GetUsersByRoleQueryResult
            {
                Users = _mapper.Map<List<UserDto>>(users)
            };

            return result;
        }
    }
}

