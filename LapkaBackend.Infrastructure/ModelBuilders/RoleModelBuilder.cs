using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Infrastructure.Data;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class RoleModelBuilder
    {
        public static void BuildRoleModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(eb =>
            {
                eb.Property(x => x.RoleName)
                    .IsRequired();

                eb.HasMany(r => r.Users)
                    .WithOne(u => u.Role);
            });
        }

        public static void SeedRoles(DbContextOptions<DataContext> options)
        {
            var dbContext = new DataContext(options);

            if (!dbContext.Roles.Any())
            {
                var roleList = new List<Role>()
                {
                    new () { RoleName = "Undefined" },
                    new () { RoleName = "SuperAdmin" },
                    new () { RoleName = "Admin" },
                    new () { RoleName = "User" },
                    new () { RoleName = "Shelter" },
                    new () { RoleName = "Worker" },
                };

                dbContext.Roles.AddRange(roleList);
                dbContext.SaveChanges();
            }
        }
    }
}
