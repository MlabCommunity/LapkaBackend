using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class UserRegistrationRequest
{
    [MinLength(1)]
    public string? FirstName { get; set; }
    [MinLength(1)]
    public string? LastName { get; set; }
    [MinLength(1)]
    public string? Email { get; set; }
    [MinLength(1)]
    public string? Password { get; set; }
    [MinLength(1)]
    public string? ConfirmPassword { get; set; }
}