using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Domain.Entities
{
    public class Shelter
    {
        public string OrganizationName { get; set; }
        public float Longtitude { get; set; }
        public float Latitude { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Nip { get; set; }
        public string Krs { get; set; }
        public string phoneNumber { get; set; }

    }
}
