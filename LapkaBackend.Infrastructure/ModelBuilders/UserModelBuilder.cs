using System.Security.Cryptography;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using LapkaBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class UserModelBuilder
    {
        public static void BuildUserModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.ToTable("Users")
                    .HasKey(e => e.Id);

                u.HasOne(e => e.Role)
                    .WithMany(e => e.Users);
                    
                u.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsRequired();
            });

        }
        
        public static void SeedUser(DbContextOptions<DataContext> options)
        {
            var dbContext = new DataContext(options);

            if (!dbContext.Users.Any())
            {
                var userList = new List<User>()
                {
                    new () { 
                        FirstName = "Super", 
                        LastName = "Admin", 
                        Email = "lappka2k23@gmail.com", 
                        Password = "$2a$12$T2jq9LwWyKjGZ5k8u.eNz..NIWCeTTc7p3vykFrRjoy9vX7VMo47O",
                        Role = dbContext.Roles.First(x => x.RoleName == Roles.SuperAdmin.ToString()),
                        VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
                        VerifiedAt = DateTime.UtcNow
                    }
                };

                dbContext.Users.AddRange(userList);
                dbContext.SaveChanges();
            }
        }
    }
}
