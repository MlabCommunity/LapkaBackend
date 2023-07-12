using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos
{
    public class UserLoginDto
    {
        //TODO: Rozdzielić wszystkie DTO/Requesty tak jak w starym Api 
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
