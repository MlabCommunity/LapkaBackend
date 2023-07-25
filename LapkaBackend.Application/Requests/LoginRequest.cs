using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
    }
}
