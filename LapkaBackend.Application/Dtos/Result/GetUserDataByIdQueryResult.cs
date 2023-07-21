namespace LapkaBackend.Application.Dtos.Result
{
    public class GetUserDataByIdQueryResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }

    }
}
