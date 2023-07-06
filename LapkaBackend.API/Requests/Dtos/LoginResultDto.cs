namespace LapkaBackend.API.Requests.Dtos;

public class LoginResultDto
{
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
}