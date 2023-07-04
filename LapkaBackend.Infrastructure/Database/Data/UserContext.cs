using LapkaBackend.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Database.Data;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionStringBuilder.Build());
    }
}