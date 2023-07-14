using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos.Result
{
    public class LoginResultDto
    {
        //TODO: Gdy beda role to dodac "LoginResultWithRoleDto
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
