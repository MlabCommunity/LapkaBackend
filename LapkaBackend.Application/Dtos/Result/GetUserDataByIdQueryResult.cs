namespace LapkaBackend.Application.Dtos.Result
{
    public class GetUserDataByIdQueryResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string ProfilePicture { get; set; } = null!;

    }
}
