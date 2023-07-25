using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class TokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
