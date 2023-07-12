using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos
{
    public class ShelterRegisterDto
    {
        [Required]
        public string OrganizationName { get; set; } = string.Empty;
        [Required]
        public float Longtitude { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Street { get; set; } = string.Empty;
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        [Required]
        public string Nip { get; set; } = string.Empty;
        [Required]
        public string Krs { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
