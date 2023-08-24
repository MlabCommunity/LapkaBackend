using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddModels();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalCategory> AnimalCategories { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<AnimalView> AnimalViews { get; set; }
        public DbSet<ShelterVolunteering> SheltersVolunteering { get; set; }


        public DbSet<FileBlob> Blobs { get; set; }
    }
}
