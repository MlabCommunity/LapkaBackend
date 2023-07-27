using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class AnimalModelBuilder
    {
        public static void BuildAnimalModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>(a =>
            {
                a.ToTable("Animals")
                .HasKey(a => a.Id);

                a.Property(x => x.Name)
                .IsRequired();

                a.Property(x => x.Gender)
                .IsRequired();

                a.Property(x => x.Weight)
                .HasColumnType("decimal(5, 2)")
                .IsRequired();

                a.Property(x => x.Description)
                .IsRequired();

                a.Property(x => x.IsSterilized)
                .IsRequired();

                a.Property(x => x.Months)
                .IsRequired();


            });


        }
    }
}
