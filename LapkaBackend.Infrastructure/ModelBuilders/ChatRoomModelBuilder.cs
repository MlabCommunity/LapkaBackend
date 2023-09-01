using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapkaBackend.Infrastructure.ModelBuilders;

public class ChatRoomModelBuilder
{
    public static void BuildChatRoomModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(u =>
        {
            u.ToTable("ChatRooms")
                .HasKey(e => e.Id);

            u.HasOne(e => e.User1)
                .WithMany(e => e.ChatRoomsAsUser1)
                .HasForeignKey(e => e.User1Id);

            u.HasOne(e => e.User2)
                .WithMany(e => e.ChatRoomsAsUser2)
                .HasForeignKey(e => e.User2Id);
            
            u.HasMany(e => e.Messages)
                .WithOne(e => e.Room)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
        });
    }
}