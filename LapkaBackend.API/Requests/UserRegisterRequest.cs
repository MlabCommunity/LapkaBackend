using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class UserRegistrationRequest : LoginRequest
{
    [MinLength(1)]
    public string? FirstName { get; set; }
    [MinLength(1)]
    public string? LastName { get; set; }
    [MinLength(1)]
    public string? ConfirmPassword { get; set; }
}