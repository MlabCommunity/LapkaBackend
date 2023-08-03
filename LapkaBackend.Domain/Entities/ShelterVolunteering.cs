using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class ShelterVolunteering
    {
        [Key, ForeignKey("ShelterId")]
        public Guid ShelterId { get; set; }
        public virtual Shelter? Shelter { get; set; }

        public bool IsDonationActive { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? DonationDescription { get; set; }
        public bool IsDailyHelpActive { get; set; }
        public string? DailyHelpDescription { get; set; }
        public bool IsTakingDogsOutActive { get; set; }
        public string? TakingDogsOutDesctiption { get; set; }
    }
}
