using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Data
{
    public class DataContext : DbContext, IDataContext
    {
        
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddModels();
            modelBuilder.Seed();

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<AnimalCategory> AnimalCategories { get; set; }


    }
}
