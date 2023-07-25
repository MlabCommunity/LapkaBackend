using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class UserEmailRequest
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
