using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Entities;


namespace LapkaBackend.Application.Services;

public class UserService : IUserService
{
    private readonly ITokenService _tokenService = new TokenService();
    
    public async Task<User> LoginMobile(IDataContext context, Dictionary<string, string> credentials)
    {
        var userDb = context.Users.FirstOrDefault(x => 
            x.Email == credentials["email"] && x.Password == credentials["password"]);
        if (userDb == null) return null;

        userDb.RefreshToken = _tokenService.GenerateRefreshToken();
        await context.SaveChangesAsync();

        userDb.AccessToken = _tokenService.GenerateAccessToken();
        return userDb;
    }

    public async Task<bool> LoginWeb(IDataContext context, Dictionary<string, string> credentials)
    {
        throw new NotImplementedException();
    }

    
    public async Task<bool> Register(IDataContext context, Dictionary<string, string> credentials)
    {
        if (context.Users.Any(x => x.Email == credentials["email"])) return false;
        if (credentials["password"] != credentials["confirmPassword"]) return false;
        context.Users.Add(new User
        {
            FirstName = credentials["firstName"],
            LastName = credentials["lastName"],
            Email = credentials["email"],
            Password = credentials["password"],
            RefreshToken = _tokenService.GenerateRefreshToken()
        });
        await context.SaveChangesAsync();
        return true;
    }

}