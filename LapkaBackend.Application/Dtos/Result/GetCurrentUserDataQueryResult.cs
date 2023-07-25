using LapkaBackend.Domain.Enums;

namespace LapkaBackend.Application.Dtos.Result
{
    public class GetCurrentUserDataQueryResult
    {
        //TODO: Dodać login provider po swagger review
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        //public string ProfilePicture { get; set; }
        public Roles Role { get; set; }
        //public string LoginProvider { get; set; }

    }
}
