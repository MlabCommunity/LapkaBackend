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
    
    public async Task<User> LoginMobile(IDataContext context, Credentials credentials)
    {
        var userDb = context.Users.FirstOrDefault(x => 
            x.Email == credentials.Email && x.Password == credentials.Password);
        if (userDb is null) 
            throw new PlaceholderException("User not found");

        userDb.RefreshToken = _tokenService.GenerateRefreshToken();
        await context.SaveChangesAsync();

        userDb.AccessToken = _tokenService.GenerateAccessToken(userDb);
        return userDb;
    }

    public async Task<bool> LoginWeb(IDataContext context, Credentials credentials)
    {
        throw new NotImplementedException();
    }

    
    public async Task Register(IDataContext context, Credentials credentials)
    {
        if (context.Users.Any(x => x.Email == credentials.Email)) 
            throw new PlaceholderException("User already exists");
        if (credentials.Password != credentials.ConfirmPassword)
            throw new PlaceholderException("Passwords do not match");
        context.Users.Add(new User
        {
            FirstName = credentials.FirstName,
            LastName = credentials.LastName,
            Email = credentials.Email,
            Password = credentials.Password, //TODO Hashing passwords
            RefreshToken = _tokenService.GenerateRefreshToken()
        });
        await context.SaveChangesAsync();
    }
    // what to do with this direcotry
    // we can just create models that will have only one task to transport data
    // but it would be pretty much the same thing as it is now key-value pairs
    // maybe try to transport data just in this requests it can be good but how to get them
    // if application layer cannot access api layer
    // or we can just make class credentials to pretty much simulate the directory but with nullable fields
    // in every request we just assign necessary fields and pass to service 
    // it looks good and we preserve as much as possible from the old code

}