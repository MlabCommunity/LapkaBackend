using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class SetNewPasswordRequest
    {
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}
