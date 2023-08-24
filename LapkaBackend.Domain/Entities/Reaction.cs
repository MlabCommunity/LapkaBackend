namespace LapkaBackend.Domain.Entities
{
    public class Reaction
    {
        public Guid Id { get; set; }
        public string NameOfReaction { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public virtual required User User { get; set; }

        public Guid AnimalId { get; set; }
        public virtual required Animal Animal { get; set; }
    }
}
