namespace LapkaBackend.Application.Dtos.Result;

public class LoginResultWithRoleDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    // we need to make role enum to replicate the old api
}