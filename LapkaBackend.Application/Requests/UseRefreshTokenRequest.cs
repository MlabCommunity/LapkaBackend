using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.Application.Requests
{
    public class UseRefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; } = null!;
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
