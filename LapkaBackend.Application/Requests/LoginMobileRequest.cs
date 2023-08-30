namespace LapkaBackend.Application.Requests;

public class LoginMobileRequest : LoginRequest
{
    public string RegistrationToken { get; set; } = string.Empty;   
}