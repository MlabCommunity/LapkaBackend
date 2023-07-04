using System.ComponentModel.DataAnnotations;

namespace LapkaBackend.API.Requests;

public class UserRegistrationRequest
{
    [MinLength(1)]
    public string firstName { get; set; }
    [MinLength(1)]
    public string lastName { get; set; }
    [MinLength(1)]
    public string email { get; set; }
    [MinLength(1)]
    public string password { get; set; }
    [MinLength(1)]
    public string passwordConfirm { get; set; }
}