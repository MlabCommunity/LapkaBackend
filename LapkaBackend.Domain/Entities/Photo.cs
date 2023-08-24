namespace LapkaBackend.Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public bool IsProfilePhoto { get; set; }

        public Guid? AnimalId { get; set; }
        public virtual Animal Animal { get; set; }
    }
}
