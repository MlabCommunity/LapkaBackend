namespace LapkaBackend.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public virtual List<User> Users { get; set; }
    }

}
