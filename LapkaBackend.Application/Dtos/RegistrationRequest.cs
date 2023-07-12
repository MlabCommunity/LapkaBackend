using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos
{
    public class RegistrationRequest
    {
        public ShelterRegisterDto ShelterDto { get; set; }
        public UserRegisterDto UserDto { get; set; }
    }
}
