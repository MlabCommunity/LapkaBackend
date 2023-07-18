namespace LapkaBackend.Application.Dtos.Result;

public class LoginResultWithRoleDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
    // we need to make role enum to replicate the old api
}