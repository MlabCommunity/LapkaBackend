using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Dtos
{
    public class TokensDto
    {
        [Required]
        public string AccessToken { get; set; } = null!;
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
