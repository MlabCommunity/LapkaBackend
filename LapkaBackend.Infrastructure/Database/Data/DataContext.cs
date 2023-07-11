using LapkaBackend.Application;
using LapkaBackend.Domain;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Infrastructure.Database.ModelBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LapkaBackend.Infrastructure.Database.Data;

public class DataContext : DbContext, IDataContext
{
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration) : base()
    {
        _configuration = configuration;
    }

    public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Shelter> Shelters { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration["Sql:String"]);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        UserModelBuilder.Build(modelBuilder);
    }
}