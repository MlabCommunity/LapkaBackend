using LapkaBackend.Application;
using LapkaBackend.Domain;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Database.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() : base() {}
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Shelter> Shelters { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString.GetString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().Property("Id").ValueGeneratedOnAdd();
        modelBuilder.Entity<User>().Property("FirstName").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("LastName").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("Email").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("Password").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("RefreshToken").HasColumnType("nvarchar(max)");
        modelBuilder.Entity<User>().ToTable("Users");

    }
}