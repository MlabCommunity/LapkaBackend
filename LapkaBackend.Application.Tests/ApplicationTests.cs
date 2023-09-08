using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Interfaces;
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
        var userMock = _proxyGenerator.CreateClassProxy<User>();
        userMock.FirstName = "John";
        userMock.LastName = "Doe";
        userMock.Email = "john@example.com";
        userMock.Password = BCrypt.Net.BCrypt.HashPassword("zaq1@WSX");
        userMock.VerificationToken = "token";
        userMock.Role = new Role { RoleName = Roles.User.ToString() };

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

    //TODO: Validation when access token is just a string, not a JWT token in RefreshAccessToken method
    [Test]
    public void Test_VerifyEmail_WithCorrectToken_ResultSetVerifiedAtInDataBaseOnCurrentDate()
    {
        _userService.VerifyEmail("token").Wait();
        Assert.That(_dbContext.Object.Users.First().VerifiedAt, Is.Not.Null);
    }

    [Test]
    public void Test_VerifyEmail_WithIncorrectToken_ResultBadRequestException()
    {
        Assert.ThrowsAsync<BadRequestException>(async () => await _userService.VerifyEmail("wrong_token"));
    }

    [Test]
    public void Test_LoginUser_WithWrongEmail_ResultBadRequestException()
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
    public void Test_LoginUser_WithNotVerifiedMail_ResultForbiddenException()
    {
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginUser(credentials));
    }

    [Test]
    public void Test_LoginUser_WithWrongPassword_ResultBadRequestException()
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
    public void Test_LoginUser_WithShelterRole_ResultBadRequestException()
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
    public void Test_LoginUser_WithUndefinedRole_ResultBadRequestException()
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
    public void Test_LoginUser_WithCorrectData_ResultLoginResultDto()
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
    public void Test_RegisterUser_WithExistingEmail_ResultBadRequestException()
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
    public void Test_LoginShelter_WithWrongEmail_ResultBadRequestException()
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
    public void Test_LoginShelter_WithNotVerifiedMail_ResultForbiddenException()
    {
        var credentials = new LoginMobileRequest
        {
            Email = "john@example.com",
            Password = "zaq1@WSX"
        };

        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginShelter(credentials));
    }

    [Test]
    public void Test_LoginShelter_WithWrongPassword_ResultBadRequestException()
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
    public void Test_LoginShelter_WithUserRole_ResultBadRequestException()
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
    public void Test_LoginShelter_WithUndefinedRole_ResultBadRequestException()
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
    public void Test_LoginShelter_WithCorrectData_ResultLoginResultDto()
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
    public void Test_RefreshAccessToken_WithInvalidAccessToken_ResultBadRequestException()
    {
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateRefreshToken(), // wrong token
            RefreshToken = _authService.CreateRefreshToken()
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RefreshAccessToken(credentials));
    }

    [Test]
    public void Test_RefreshAccessToken_WithInvalidRefreshToken_ResultBadRequestException()
    {
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateAccessToken(_dbContext.Object.Users.First()),
            RefreshToken = "wrong_token"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RefreshAccessToken(credentials));
    }

    [Test]
    public void Test_RefreshAccessToken_WithCorrectData_ResultUserRefreshTokenDtoIsNotNull()
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
    public void Test_CreateAccessToken_WithCorrectAccessToken_ResultStringIsNotNull()
    {
        var expected = _authService.CreateAccessToken(_dbContext.Object.Users.First());

        Assert.That(expected, Is.Not.Null);
    }

    [Test]
    public void Test_CreateRefreshToken_WithCorrectRefreshToken_ResultStringIsNotNull()
    {
        var expected = _authService.CreateRefreshToken();

        Assert.That(expected, Is.Not.Null);
    }

    [Test]
    public void Test_IsTokenValid_WithWrongToken_ResultFalse()
    {
        var expected = _authService.IsTokenValid("wrong_token");

        Assert.That(expected, Is.False);
    }

    [Test]
    public void Test_IsTokenValid_WithCorrectRefreshToken_ResultTrue()
    {
        var expected = _authService.IsTokenValid(_authService.CreateRefreshToken());

        Assert.That(expected, Is.True);
    }

    [Test]
    public void Test_IsTokenValid_WithCorrectAccessToken_ResultTrue()
    {
        var expected = _authService.IsTokenValid(_authService.CreateAccessToken(_dbContext.Object.Users.First()));

        Assert.That(expected, Is.True);
    }

    [Test]
    public void Test_RevokeToken_WithIncorrectRefreshToken_ResultBadRequestException()
    {
        var credentials = new TokenRequest
        {
            RefreshToken = "wrong_token"
        };

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RevokeToken(credentials));
    }

    [Test]
    public void Test_ResetPassword_WithIncorrectEmail_ResultBadRequestException()
    {
        var credentials = new UserEmailRequest
        {
            Email = "wrong_email@example.com"
        };
        
        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.ResetPassword(credentials));
    }
    
    [Test]
    public void Test_SetNewPassword_WithIncorrectToken_ResultBadRequestException()
    {
        var credentials = new ResetPasswordRequest
        {
            Password = "zaq1@WSX",
            ConfirmPassword = "zaq1@WSX"
        };
        
        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.SetNewPassword(credentials, "wrong_token"));
    }
}