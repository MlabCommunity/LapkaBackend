using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class AnimalViewsModelBuilder
    {
        public static void BuildAnimalViewModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimalView>(ac =>
            {
                ac.Property(x => x.ViewDate)
                .IsRequired();

                ac.HasOne(r => r.Animal)
                .WithMany(u => u.AnimalViews)
                .HasForeignKey(x => x.AnimalId);

                ac.HasOne(r => r.User)
                .WithMany(u => u.AnimalViews)
                .HasForeignKey(x => x.UserId);
            });
        }
    }
}
