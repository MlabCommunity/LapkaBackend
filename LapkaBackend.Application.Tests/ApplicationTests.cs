using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Tests.Helper;
using LapkaBackend.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace LapkaBackend.Application.Tests;

public class Tests
{
    private Mock<IDataContext> _dbContext;
    private Mock<IConfiguration> _configuration;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<IEmailService> _emailService;
    private Mock<ILogger> _logger;
    private AuthService _authService;
    private UserService _userService;

    private readonly ProxyGenerator _proxyGenerator = new();

    [SetUp]
    public void Setup()
    {
        //TODO Move it to individual tests and use only what is needed
        var userMock = new UserMockBuilder()
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithEmail("john@example.com")
            .WithPassword("zaq1@WSX")
            .WithVerificationToken("token")
            .WithRole(new Role { RoleName = Roles.User.ToString() })
            .Build();

        var usersMock = Mocker.MockDbSet(userMock);

        _dbContext = new Mock<IDataContext>();
        _dbContext.Setup(dc => dc.Users).Returns(usersMock.Object);

        var tokenMock = new Mock<IConfigurationSection>();
        tokenMock.SetupGet(t => t.Value).Returns("ulpgOBFxsvUAT4jYHYPyzAyfphyivEDB");

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(conf => conf.GetSection("AppSettings:Token")).Returns(tokenMock.Object);

        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

        _emailService = new Mock<IEmailService>();
        _emailService.Setup(x => x.SendEmail(It.IsAny<MailRequest>())).Returns(Task.CompletedTask);

        _logger = new Mock<ILogger>();
        _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()));

        _authService = new AuthService(_dbContext.Object,
            _configuration.Object, _emailService.Object, _httpContextAccessor.Object, _logger.Object);

        _userService = new UserService(_dbContext.Object, _emailService.Object, null!, _httpContextAccessor.Object);
    }

    [Test]
    public void VerifyEmail_WithCorrectToken_SetsVerifiedAtInDatabaseToCurrentDate()
    {
        _userService.VerifyEmail("token").Wait();
        Assert.That(_dbContext.Object.Users.First().VerifiedAt, Is.Not.Null);
    }

    [Test]
    public void VerifyEmail_WithIncorrectToken_ThrowsBadRequestException()
    {
        Assert.ThrowsAsync<BadRequestException>(async () => await _userService.VerifyEmail("wrong_token"));
    }

    [Test]
    public void LoginUser_WithWrongEmail_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        var credentials = new LoginMobileRequest
        {
            Email = "wrongMail@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void LoginUser_WithNotVerifiedMail_ThrowsForbiddenException()
    {
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void LoginUser_WithWrongPassword_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "wrong_password"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void LoginUser_WithShelterRole_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        _dbContext.Object.Users.First().Role = new Role { RoleName = Roles.Shelter.ToString() };

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void LoginUser_WithUndefinedRole_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        _dbContext.Object.Users.First().Role = new Role { RoleName = Roles.Undefined.ToString() };

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void LoginUser_WithCorrectData_ReturnsLoginResultDto()
    {
        _userService.VerifyEmail("token").Wait();
        _dbContext.Object.Users.First().RefreshToken = _authService.CreateRefreshToken();

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        var result = _authService.LoginUser(credentials).Result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null);
        });
    }

    [Test]
    public void RegisterUser_WithExistingEmail_ThrowsBadRequestException()
    {
        var credentials = new UserRegistrationRequest
        {
            FirstName = "John",
            LastName = "Doe",
            EmailAddress = "john@example.com",
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RegisterUser(credentials));
    }

    [Test]
    public void LoginShelter_WithWrongEmail_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        var credentials = new LoginMobileRequest
        {
            Email = "wrongMail@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void LoginShelter_WithNotVerifiedMail_ThrowsForbiddenException()
    {
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void LoginShelter_WithWrongPassword_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "wrong_password"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void LoginShelter_WithUserRole_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void LoginShelter_WithUndefinedRole_ThrowsBadRequestException()
    {
        _userService.VerifyEmail("token").Wait();
        _dbContext.Object.Users.First().Role = new Role { RoleName = Roles.Undefined.ToString() };

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void LoginShelter_WithCorrectData_ReturnsLoginResultDto()
    {
        _userService.VerifyEmail("token").Wait();
        _dbContext.Object.Users.First().RefreshToken = _authService.CreateRefreshToken();
        _dbContext.Object.Users.First().Role = new Role { RoleName = Roles.Shelter.ToString() };

        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        var result = _authService.LoginShelter(credentials).Result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null);
        });
    }

    [Test]
    public void RefreshAccessToken_WithInvalidAccessToken_ThrowsBadRequestException()
    {
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateRefreshToken(), // wrong token
            RefreshToken = _authService.CreateRefreshToken()
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RefreshAccessToken(credentials));
    }

    [Test]
    public void RefreshAccessToken_WithInvalidRefreshToken_ThrowsBadRequestException()
    {
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateAccessToken(_dbContext.Object.Users.First()),
            RefreshToken = "wrong_token"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RefreshAccessToken(credentials));
    }

    [Test]
    public void RefreshAccessToken_WithCorrectData_ReturnsUserRefreshTokenDtoIsNotNull()
    {
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateAccessToken(_dbContext.Object.Users.First()),
            RefreshToken = _authService.CreateRefreshToken()
        };

        var expected = _authService.RefreshAccessToken(credentials).Result;

        Assert.Multiple(() =>
        {
            Assert.That(expected, Is.Not.Null);
            Assert.That(expected.AccessToken, Is.Not.Null);
        });
    }

    [Test]
    public void CreateAccessToken_WithCorrectAccessToken_ReturnsStringIsNotNull()
    {
        var expected = _authService.CreateAccessToken(_dbContext.Object.Users.First());

        Assert.That(expected, Is.Not.Null);
    }

    [Test]
    public void CreateRefreshToken_WithCorrectRefreshToken_ReturnsStringIsNotNull()
    {
        var expected = _authService.CreateRefreshToken();

        Assert.That(expected, Is.Not.Null);
    }

    [Test]
    public void IsTokenValid_WithWrongToken_ReturnsFalse()
    {
        var expected = _authService.IsTokenValid("wrong_token");

        Assert.That(expected, Is.False);
    }

    [Test]
    public void IsTokenValid_WithCorrectRefreshToken_ReturnsTrue()
    {
        var expected = _authService.IsTokenValid(_authService.CreateRefreshToken());

        Assert.That(expected, Is.True);
    }

    [Test]
    public void IsTokenValid_WithCorrectAccessToken_ReturnsTrue()
    {
        var expected = _authService.IsTokenValid(_authService.CreateAccessToken(_dbContext.Object.Users.First()));

        Assert.That(expected, Is.True);
    }

    [Test]
    public void RevokeToken_WithIncorrectRefreshToken_ThrowsBadRequestException()
    {
        var credentials = new TokenRequest
        {
            RefreshToken = "wrong_token"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RevokeToken(credentials));
    }

    [Test]
    public void ResetPassword_WithIncorrectEmail_ThrowsBadRequestException()
    {
        var credentials = new UserEmailRequest
        {
            Email = "wrong_email@example.com"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.ResetPassword(credentials));
    }

    [Test]
    public void SetNewPassword_WithIncorrectToken_ThrowsBadRequestException()
    {
        var credentials = new ResetPasswordRequest
        {
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX"
        };

        Assert.ThrowsAsync<BadRequestException>(async () =>
            await _authService.SetNewPassword(credentials, "wrong_token"));
    }
}