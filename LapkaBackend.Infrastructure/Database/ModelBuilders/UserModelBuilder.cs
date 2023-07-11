using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.Database.ModelBuilders;

public class UserModelBuilder : DbContext
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property("Id").ValueGeneratedOnAdd().IsRequired();
        modelBuilder.Entity<User>().Property("FirstName").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("LastName").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("Email").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("Password").HasColumnType("nvarchar(max)").IsRequired();
        modelBuilder.Entity<User>().Property("RefreshToken").HasColumnType("nvarchar(max)");
        modelBuilder.Entity<User>().ToTable("Users");
    }
}