namespace LapkaBackend.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string ExternalToken { get; set; } = string.Empty;
        public Role Role { get; set; }
        public int RoleId { get; set; }
    }
}
