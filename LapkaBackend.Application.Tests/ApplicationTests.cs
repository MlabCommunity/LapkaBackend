using System.Net;
using LapkaBackend.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LapkaBackend.Application.Tests;

public class Tests
{
    private Mock<IDataContext> _dbContext;
    private Mock<IConfiguration> _configuration;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
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
        
        var usersMock = Mocker.MockDbSet(userMock);

        _dbContext = new Mock<IDataContext>();
        _dbContext.Setup(dc => dc.Users).Returns(usersMock.Object);

        var tokenMock = new Mock<IConfigurationSection>();
        tokenMock.SetupGet(t => t.Value).Returns("ulpgOBFxsvUAT4jYHYPyzAyfphyivEDB");

        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(conf => conf.GetSection("AppSettings:Token")).Returns(tokenMock.Object);

        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        
        _authService = new AuthService(_dbContext.Object,
            _configuration.Object, null!, _httpContextAccessor.Object, null!);

        _userService = new UserService(_dbContext.Object, null!, null!, null!);
    }

    [Test]
    public void Test_VerifyEmail_WithCorrectToken_SetVerifiedAtInDataBaseOnCurrentDate()
    {
        _userService.VerifyEmail("token").Wait();
        Assert.That(_dbContext.Object.Users.First().VerifiedAt, Is.Not.Null);
    }

    [Test]
    public void Test_VerifyEmail_WithInCorrectToken_ResultBadRequestException()
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
        _dbContext.Object.Users.First().Role = new Role { RoleName = Roles.User.ToString() };
        
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
    
}