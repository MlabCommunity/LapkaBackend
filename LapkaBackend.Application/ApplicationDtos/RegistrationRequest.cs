using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.ApplicationDtos
{
    public class RegistrationRequest
    {
        public ShelterDto ShelterDto { get; set; }
        public UserDto UserDto { get; set; }
    }
}
