using System.ComponentModel.DataAnnotations.Schema;

namespace LapkaBackend.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? RefreshToken { get; set; }
    [NotMapped]
    public string AccessToken { get; set; } = null!;
}