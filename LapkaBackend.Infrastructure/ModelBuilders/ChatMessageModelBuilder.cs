using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders;

public class ChatMessageModelBuilder
{
    public static void BuildChatMessageModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(u =>
        {
            u.ToTable("ChatMessages")
                .HasKey(e => e.Id);

            u.HasOne(e => e.Room)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.RoomId);

            u.HasOne(e => e.User)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.UserId);
        });

    }
}