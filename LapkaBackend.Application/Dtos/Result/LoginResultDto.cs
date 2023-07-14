namespace LapkaBackend.Application.Dtos.Result
{
    public class LoginResultDto
    {
        //TODO: Gdy beda role to dodac "LoginResultWithRoleDto
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
