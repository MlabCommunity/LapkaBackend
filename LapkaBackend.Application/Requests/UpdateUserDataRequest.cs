using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Requests
{
    public class UpdateUserDataRequest
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public string? ProfilePicture { get; set; }
    }
}
