using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Application.Common
{
    public interface IDataContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Shelter> Shelters { get; }
        public DbSet<Role> Roles { get; }
        public DbSet<Animal> Animals { get; }
        public DbSet<AnimalCategory> AnimalCategories { get; }
        public DbSet<Photo> Photos { get; }
        public DbSet<Reaction> Reactions { get; }
        public DbSet<AnimalView> AnimalViews { get; }
        public DbSet<ShelterVolunteering> SheltersVolunteering { get; }
        public DbSet<FileBlob> Blobs { get; }



        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
