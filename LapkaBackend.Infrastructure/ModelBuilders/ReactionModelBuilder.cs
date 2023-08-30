using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders
{
    public class ReactionModelBuilder
    {
        public static void BuildReactionModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reaction>(ac =>
            {
                ac.Property(x => x.NameOfReaction)
                .IsRequired();

                ac.HasOne(r => r.User)
                .WithMany(u => u.Reactions);

                ac.HasOne(r => r.Animal)
                .WithMany(u => u.Reactions);
            });
        }
    }
}
