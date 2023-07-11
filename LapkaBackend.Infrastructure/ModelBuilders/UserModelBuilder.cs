using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class UserModelBuilder
    {
        public static void BuildUserModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasAnnotation("RegularExpression", "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$");
        }
    }
}
