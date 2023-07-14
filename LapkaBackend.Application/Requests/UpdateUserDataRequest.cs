using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class UpdateUserDataRequest
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string? ProfilePicture { get; set; }
    }
}
