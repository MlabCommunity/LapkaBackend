namespace LapkaBackend.Application.Tests.Helper;

public class UserMockBuilder
{
    private readonly User _user = new ProxyGenerator().CreateClassProxy<User>();

    public UserMockBuilder WithFirstName(string firstName)
    {
        _user.FirstName = firstName;
        return this;
    }

    public UserMockBuilder WithLastName(string lastName)
    {
        _user.LastName = lastName;
        return this;
    }

    public UserMockBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserMockBuilder WithPassword(string password)
    {
        _user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        return this;
    }

    public UserMockBuilder WithVerificationToken(string verificationToken)
    {
        _user.VerificationToken = verificationToken;
        return this;
    }

    public UserMockBuilder WithRole(Role role)
    {
        _user.Role = role;
        return this;
    }
    //If more fields are needed add them here

    public User Build()
    {
        return _user;
    }
}