using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class TokenRequest
{
    [Required]
    public string RefreshToken { get; set; }
}