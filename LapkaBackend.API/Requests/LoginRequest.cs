using System.ComponentModel.DataAnnotations;
namespace LapkaBackend.API.Requests;

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}