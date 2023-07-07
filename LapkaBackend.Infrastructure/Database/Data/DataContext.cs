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


}