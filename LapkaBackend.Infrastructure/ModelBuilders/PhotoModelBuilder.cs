using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class PhotoModelBuilder
    {
        public static void BuildPhotoModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>(p =>
            {
                p.Property(x => x.Id)
                .IsRequired();

                p.HasOne(r => r.Animal)
                .WithMany(u => u.Photos)
                .HasForeignKey(x => x.AnimalId);
            });
        }
    }   
}
