namespace LapkaBackend.Application.Dtos.Result
{
    public class LoginResultDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
