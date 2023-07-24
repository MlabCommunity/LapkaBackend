using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class Shelter
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public float Longtitude { get; set; } 
        public float Latitude { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public string Krs { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public virtual List<Animal>? Animals { get; set; }
    }
}
