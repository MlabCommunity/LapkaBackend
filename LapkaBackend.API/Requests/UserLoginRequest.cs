using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class LoginRequest
{
    [MinLength(1)]
    public string email { get; set; }
    [MinLength(1)]
    public string password { get; set; }
}