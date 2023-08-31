
namespace LapkaBackend.Application.Tests;

public class Tests
{
    private Mock<IDataContext> _dbContext;
    private Mock<IConfiguration> _configuration;
    private AuthService _authService;
    private UserService _userService;
    private readonly ProxyGenerator _proxyGenerator = new();

    [SetUp]
    public void Setup()
    {
        var userMock = _proxyGenerator.CreateClassProxy<User>();
        userMock.FirstName = "John";
        userMock.LastName = "Doe";
        userMock.Email = "john@example.com";
        userMock.Password = "hashed_password";
        userMock.VerificationToken = "token";

        var usersMock = Mocker.MockDbSet(userMock);

        _dbContext = new Mock<IDataContext>();
        _dbContext.Setup(dc => dc.Users).Returns(usersMock.Object);

        var tokenMock = new Mock<IConfigurationSection>();
        tokenMock.SetupGet(t => t.Value).Returns("ulpgOBFxsvUAT4jYHYPyzAyfphyivEDB");

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(conf => conf.GetSection("AppSettings:Token")).Returns(tokenMock.Object);

        _authService = new AuthService(_dbContext.Object,
            _configuration.Object, null!, null!, null!);

        _userService = new UserService(_dbContext.Object, null!, null!, null!);
    }

    [Test]
    public void TestUserLoginNotVerified()
    {
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "hashed_password"
        };

        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void TestUserVerifying()
    {
        _userService.VerifyEmail("token").Wait();
        Assert.That(_dbContext.Object.Users.First().VerifiedAt, Is.Not.Null);
    }
}