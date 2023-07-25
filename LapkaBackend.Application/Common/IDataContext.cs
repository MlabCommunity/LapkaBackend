using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Common
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Shelter> Shelters { get; }
        public DbSet<Role> Roles { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
