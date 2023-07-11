namespace LapkaBackend.API.Requests.Dtos;

public class LoginResultDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}