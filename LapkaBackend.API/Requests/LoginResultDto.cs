namespace LapkaBackend.API.Requests;

public class LoginResultDto
{
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
}