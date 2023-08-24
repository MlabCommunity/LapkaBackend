namespace LapkaBackend.Domain.Entities
{
    public class AnimalCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public virtual List<Animal> Animals { get; set; }
    }
}
