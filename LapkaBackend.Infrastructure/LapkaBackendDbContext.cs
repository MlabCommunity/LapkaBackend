using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure
{
    public class LapkaBackendDbContext : DbContext, ILapkaBackendDbContext
    {
        private string _connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=LapkaBackend;Trusted_Connection=True;TrustServerCertificate=true;";
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(r => r.FirstName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<User>()
                .Property(r => r.LastName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<User>()
                .Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(25);
            modelBuilder.Entity<User>()
                .Property(r => r.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
            .Property(r => r.PasswordSalt)
            .IsRequired();


            modelBuilder.Entity<Shelter>()
                .Property(r => r.OrganizationName)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(r => r.City)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(r => r.Street)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(r => r.ZipCode)
                .IsRequired();

            modelBuilder.Entity<Shelter>()
                .Property(r => r.Nip)
                .IsRequired();
        }   

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LapkaBackend;Trusted_Connection=true;TrustServerCertificate=true;");
        }

        public LapkaBackendDbContext(DbContextOptions<LapkaBackendDbContext> options) : base(options)
        {

        }
    }
}
