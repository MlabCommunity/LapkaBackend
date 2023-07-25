using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class UpdateUserEmailRequest
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
