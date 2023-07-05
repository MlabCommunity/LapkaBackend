using System.ComponentModel.DataAnnotations;
namespace LapkaBackend.API.Requests;

public class LoginRequest
{
    [MinLength(1)]
    public string? Email { get; set; }
    [MinLength(1)]
    public string? Password { get; set; }
}