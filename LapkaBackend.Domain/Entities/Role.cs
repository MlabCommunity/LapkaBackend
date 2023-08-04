namespace LapkaBackend.Domain.Entities
{
    public sealed class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public List<User>? Users { get; set; }
    }

}
