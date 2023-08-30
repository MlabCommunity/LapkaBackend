﻿namespace LapkaBackend.Domain.Entities
{
    public class Animal
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Marking { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public bool IsArchival { get; set; }
        public int CategoryId { get; set; }
        public virtual AnimalCategory AnimalCategory { get; set; }

        public Guid ShelterId { get; set; }
        public virtual Shelter Shelter { get; set; }

        public string? ProfilePhoto { get; set; }

        public virtual List<Reaction> Reactions { get; set; }

        public virtual List<AnimalView>? AnimalViews { get; set; }
    }
}
