using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class RoleModelBuilder
    {
        public static void BuildRoleModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
            .ToTable("Roles")
            .HasKey(u => u.Id);

            modelBuilder.Entity<Role>()
                .Property(u => u.RoleName)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}
