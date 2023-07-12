using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
                .Property(u => u.Longtitude)
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
                 .IsRequired()
                .HasAnnotation("RegularExpression", "^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
        }
    }
}
