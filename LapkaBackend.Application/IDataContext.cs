using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace LapkaBackend.Application;

public interface IDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Shelter> Shelters { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}