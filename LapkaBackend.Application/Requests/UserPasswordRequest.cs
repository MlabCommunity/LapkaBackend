using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class UserPasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
