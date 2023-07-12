using LapkaBackend.Application.Common;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // TODO: Puszczenie migracji po dodaniu wszystkich atrybutów
            modelBuilder.AddModels();

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Shelter> Shelters { get; set; }


    }
}
