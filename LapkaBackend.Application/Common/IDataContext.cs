using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Common
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Shelter> Shelters { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<FileBlob> Blobs { get; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalCategory> AnimalCategories { get; set; }



        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
