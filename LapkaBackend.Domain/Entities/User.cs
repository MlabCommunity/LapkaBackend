﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LapkaBackend.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        public Guid ShelterId { get; set; }
        public virtual Shelter? Shelter { get; set; }

        public virtual List<Reaction>? Reactions { get; set; }

        public string LoginProvider { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;

        public virtual List<AnimalView>? AnimalViews { get; set; }
    }
}
