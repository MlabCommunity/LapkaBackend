using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class ShelterVolunteeringModelBuilder
    {
        public static void BuildShelterVolunteeringModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShelterVolunteering>(a =>
            {
                a.ToTable("SheltersVolunteering")
                .HasKey(a => a.ShelterId);

                a.HasOne(s => s.Shelter)
                .WithOne(s => s.ShelterVolunteering);

                a.Property(s => s.BankAccountNumber)
                .IsRequired();
            });
        }
    }
}
