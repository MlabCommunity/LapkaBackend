using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class ShelterModelBuilder
    {
        public static void BuildShelterModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shelter>()
            .ToTable("Shelters")
            .HasKey(u => u.Id);

            modelBuilder.Entity<Shelter>()
                .Property(u => u.OrganizationName)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(u => u.Longitude)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(u => u.Latitude)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(u => u.City)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                 .Property(u => u.Street)
                 .HasMaxLength(255)
                 .IsRequired();

            modelBuilder.Entity<Shelter>()
                 .Property(u => u.ZipCode)
                 .HasMaxLength(255)
                 .IsRequired();

            modelBuilder.Entity<Shelter>()
                 .Property(u => u.Nip)
                 .HasMaxLength(255)
                 .IsRequired();

            modelBuilder.Entity<Shelter>()
                 .Property(u => u.Krs)
                 .HasMaxLength(255)
                 .IsRequired();

            modelBuilder.Entity<Shelter>()
                 .Property(u => u.PhoneNumber)
                 .HasMaxLength(255)
                 .IsRequired();

            modelBuilder.Entity<Shelter>()
                .HasMany(e => e.Animals)
                .WithOne(e => e.Shelter)
                .HasForeignKey(e => e.ShelterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
