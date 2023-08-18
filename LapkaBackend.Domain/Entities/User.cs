using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? RoleId { get; set; }
        public  Role Role { get; set; }
        public Guid? ShelterId { get; set; }
        public  Shelter Shelter { get; set; }
        public  List<Reaction> Reactions { get; set; }
        public string LoginProvider { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
        public DateTime? SoftDeleteAt { get; set; }
        
        public List<ChatRoom> ChatRoomsAsUser1 { get; set; }
        public List<ChatRoom> ChatRoomsAsUser2 { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public List<AnimalView> AnimalViews { get; set; }
    }
}
