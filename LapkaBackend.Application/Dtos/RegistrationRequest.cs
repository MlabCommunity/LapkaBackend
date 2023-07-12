using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos
{
    public class RegistrationRequest
    {
        [Required]
        public ShelterRegisterDto ShelterDto { get; set; } = null!;
        [Required]
        public UserRegisterDto UserDto { get; set; } = null!;
    }
}
