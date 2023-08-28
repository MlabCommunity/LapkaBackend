using EntityFrameworkCore.Testing.Common;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Requests;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MockQueryable.EntityFrameworkCore;
using Moq;

namespace LapkaBackend.Application.Tests;

public class Tests
{
    private Mock<IDataContext> _dbContext;
    private Mock<IConfiguration> _configuration;
    private AuthService _authService;
    private UserService _userService;
    [SetUp]
    public void Setup()
    {
        var roleMock = new Mock<Role>();
        roleMock.SetupGet(r => r.Id).Returns(4);
        roleMock.SetupGet(r => r.RoleName).Returns("User");
        
        var userMock = new Mock<User>();
        userMock.SetupGet(u => u.FirstName).Returns("John");
        userMock.SetupGet(u => u.LastName).Returns("Doe");
        userMock.SetupGet(u => u.Email).Returns("john@example.com");
        userMock.SetupGet(u => u.Password).Returns("hashed_password");
        userMock.SetupGet(u => u.RefreshToken).Returns("refresh_token");
        userMock.SetupGet(u => u.VerificationToken).Returns("token");
        userMock.SetupGet(u => u.RoleId).Returns(roleMock.Object.Id);
        userMock.SetupGet(u => u.Role).Returns(roleMock.Object);

        var usersMock = MockDbSet(userMock);

        var rolesMock = MockDbSet(roleMock);
        
        _dbContext = new Mock<IDataContext>();
        _dbContext.Setup(dc => dc.Users).Returns(usersMock.Object);
        _dbContext.Setup(dc => dc.Roles).Returns(rolesMock.Object);

        var tokenMock = new Mock<IConfigurationSection>();
        tokenMock.SetupGet(t => t.Value).Returns("ulpgOBFxsvUAT4jYHYPyzAyfphyivEDB");
        
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(conf => conf.GetSection("AppSettings:Token")).Returns(tokenMock.Object);
        
        _authService = new AuthService(_dbContext.Object, 
            _configuration.Object, null, null, null);

        _userService = new UserService(_dbContext.Object, null, null, null);
    }

    [Test]
    public void TestUserLoginNotVerified()
    {
        var credentials = new LoginRequest
        {
            Email = "john@example.com",
            Password = "hashed_password"
        };
        
        Assert.ThrowsAsync<ForbiddenException>(async () => await _authService.LoginUser(credentials));
    }
    
    [Test]
    public void TestUserVerifying()
    {
        _userService.VerifyEmail("token");
        Assert.That(_dbContext.Object.Users.First().VerifiedAt, Is.Not.Null);
    }
    
    private static Mock<DbSet<T>> MockDbSet<T>(IMock<T> initialObject) where T : class
    {
        var data = new List<T> { initialObject.Object }.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        
        var asyncData = new List<T> { initialObject.Object };
        var asyncQueryable = asyncData.AsQueryable();
        var asyncProvider = new AsyncQueryProvider<T>(data.AsEnumerable());
        var asyncExpression = asyncQueryable.Expression;

        mock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(asyncData.GetEnumerator()));

        mock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(asyncProvider);

        mock.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(asyncExpression);

        mock.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(asyncQueryable.ElementType);

        return mock;
    } 
    //TODO move to other class
}