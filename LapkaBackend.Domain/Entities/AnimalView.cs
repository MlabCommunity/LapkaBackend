namespace LapkaBackend.Domain.Entities
{
    public class AnimalView
    {
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        public virtual Animal Animal { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime ViewDate { get; set; }
    }
}
