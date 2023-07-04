using System.ComponentModel.DataAnnotations.Schema;

namespace LapkaBackend.Infrastructure.Database.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    // Maybe split it in to tables to prevent updating whole user with every refresh token update
    [NotMapped]
    public string AccessToken { get; set; } = null!;
}