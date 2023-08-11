﻿using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Hangfire;

public class UpdateDeleteJob
{
    private  readonly IDataContext _dbContext;
    private readonly IBlobService _blobService;

    public UpdateDeleteJob(IDataContext dbContext, IBlobService blobService)
    {
        _dbContext = dbContext;
        _blobService = blobService;
    }

    public async Task PermDelete()
    {
        var usersToRemove = _dbContext.Users.Include(x => x.Role).Where(x => 
            x.SoftDeleteAt <= DateTime.UtcNow).ToList();
    
        foreach (var user in usersToRemove)
        {
            await _blobService.DeleteFilesByParentId(user.Id);
            _dbContext.Users.Remove(user);
            if (user.Role.RoleName == Roles.Shelter.ToString())
            {
                var shelterUsers = _dbContext.Users.
                    Where(x => x.ShelterId == user.ShelterId && x.Role.RoleName == Roles.Worker.ToString()).ToList();
    
                foreach (var worker in shelterUsers)
                {
                    worker.Role = _dbContext.Roles.First(x => x.RoleName == Roles.User.ToString());
                    worker.ShelterId = Guid.Empty;
                }
                _dbContext.Shelters.Remove(_dbContext.Shelters.FirstAsync(x => x.Id == user.ShelterId).Result);
            }
            await _dbContext.SaveChangesAsync();

        }
    }
}