using LapkaBackend.Domain.Common;
using LapkaBackend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure
{
    public class LapkaBackendDBContext:DbContext, ILapkaBackendDbContext
    {
        private string _connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=LapkaBackend;Trusted_Connection=True;";
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(r => r.firstName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<User>()
                .Property(r => r.lastName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<User>()
                .Property(r => r.email)
                .IsRequired()
                .HasMaxLength(25);
        }   

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
