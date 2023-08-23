using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
                .WithOne(s => s.ShelterVolunteering)
                .HasForeignKey<ShelterVolunteering>(sv => sv.ShelterId);

                a.Property(s => s.BankAccountNumber)
                .IsRequired();
            });
        }
    }
}
