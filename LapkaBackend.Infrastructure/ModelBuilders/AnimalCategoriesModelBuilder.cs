using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LapkaBackend.Infrastructure.Data;

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
        
        public static void SeedAnimalCategories(DbContextOptions<DataContext> options)
        {
            var dbContext = new DataContext(options);

            if (!dbContext.AnimalCategories.Any())
            {
                var animalCategoriesList = new List<AnimalCategory>()
                {
                    new () { CategoryName = "Other" },
                    new () { CategoryName = "Dog" },
                    new () { CategoryName = "Cat" },
                    new () { CategoryName = "Rabbit" }                    
                };

                dbContext.AnimalCategories.AddRange(animalCategoriesList);
                dbContext.SaveChanges();
            }
        }
    }
}
