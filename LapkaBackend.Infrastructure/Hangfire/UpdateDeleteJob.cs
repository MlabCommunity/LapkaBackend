using Hangfire;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Hangfire;

public class UpdateDeleteJob
{
    private readonly IDataContext _dbContext;
    private readonly IBlobService _blobService;
    private readonly IBackgroundJobClient _jobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public UpdateDeleteJob(IDataContext dbContext, IBackgroundJobClient jobClient,
        IRecurringJobManager recurringJobManager, IBlobService blobService)
    {
        _dbContext = dbContext;
        _jobClient = jobClient;
        _recurringJobManager = recurringJobManager;
        _blobService = blobService;
    }

    public async Task PermDelete()
    {
        var usersToRemove = _dbContext.Users.Include(x => x.Role).Where(x =>
            x.SoftDeleteAt <= DateTime.UtcNow).ToList();

        foreach (var user in usersToRemove)
        {
            await _blobService.DeleteFilesByParentId(user.Id);

            var chatRooms = _dbContext.ChatRooms.Where(x => x.User1Id == user.Id ||
                                                            x.User2Id == user.Id);
            
            foreach (var chatRoom in chatRooms)
            {
                _dbContext.ChatRooms.Remove(chatRoom);
            }

            _dbContext.Users.Remove(user);

            if (user.Role.RoleName == Roles.Shelter.ToString())
            {
                var shelterUsers = _dbContext.Users
                    .Where(x => x.ShelterId == user.ShelterId && x.Role.RoleName == Roles.Worker.ToString()).ToList();

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