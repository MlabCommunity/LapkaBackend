using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class AnimalCategoryModelBuilder
    {
        public static void BuildAnimalCategoryModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimalCategory>(ac =>
            {
                ac.Property(x => x.CategoryName)
                .IsRequired();

                ac.HasMany(r => r.Animals)
                .WithOne(u => u.AnimalCategory);
            });
        }
        
    }
}
