using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.IServices;
using LapkaBackend.Domain.Entities;


namespace LapkaBackend.Application.Services;

public class UserService : IUserService
{
    private readonly ITokenService _tokenService;

    public UserService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public async Task<User> LoginMobile(IDataContext context, Dictionary<string, string> credentials)
    {
        var userDb = context.Users.FirstOrDefault(x => 
            x.Email == credentials["email"] && x.Password == credentials["password"]);
        if (userDb == default) throw new PlaceholderException("User not found");

        userDb.RefreshToken = _tokenService.GenerateRefreshToken();
        await context.SaveChangesAsync();

        userDb.AccessToken = _tokenService.GenerateAccessToken(userDb);
        return userDb;
    }

    public async Task<bool> LoginWeb(IDataContext context, Dictionary<string, string> credentials)
    {
        throw new NotImplementedException();
    }

    
    public async Task Register(IDataContext context, Dictionary<string, string> credentials)
    {
        if (context.Users.Any(x => x.Email == credentials["email"])) throw new PlaceholderException("User already exists");
        if (credentials["password"] != credentials["confirmPassword"])
            throw new PlaceholderException("Passwords do not match");
        context.Users.Add(new User
        {
            FirstName = credentials["firstName"],
            LastName = credentials["lastName"],
            Email = credentials["email"],
            Password = credentials["password"], //TODO Hashing passwords
            RefreshToken = _tokenService.GenerateRefreshToken()
        });
        await context.SaveChangesAsync();
    }

}