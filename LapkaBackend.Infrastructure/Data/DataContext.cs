using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost;Database=Lapka;Trusted_Connection=true;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // TODO: Puszczenie migracji po dodaniu wszystkich atrybutów
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);

            //modelBuilder.Entity<User>()
            //    .Property(x => x.Id).HasDefaultValue("NEWID()");

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(8)
                .HasAnnotation("RegularExpression", "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$");
        }


        public DbSet<User> Users { get; set; }


    }
}
