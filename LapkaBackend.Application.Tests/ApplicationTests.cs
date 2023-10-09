using LapkaBackend.Application.Dtos;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Application.Tests.Helper;
using LapkaBackend.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
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

    [SetUp]
    public void Setup()
    {
        //TODO Move it to individual tests and use only what is needed
        var shelterMock = new ShelterMockBuilder()
            .WithOrganizationName("Shelter")
            .WithPhoneNumber("123456789")
            .WithCity("City")
            .WithStreet("Street")
            .WithKrs("1234567890")
            .WithNip("1234567890")
            .WithZipCode("00-000")
            .WithLatitude(52.32323)
            .WithLongitude(15)
            .Build();
        
        var userMock = new UserMockBuilder()
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithEmail("john@example.com")
            .WithPassword("zaq1@WSX")
            .WithVerificationToken("token")
            .WithRole(new Role { RoleName = Roles.User.ToString() })
            .Build();
        
        
        var animalMock = new AnimalMockBuilder()
            .WithName("testAnimal")
            .WithSpecies("testSpecies")
            .WithGender("Male")
            .WithWeight((decimal)10.223232)
            .WithDescription("testDescription")
            .WithCreatedAt(DateTime.Now)
            .WithIsSterilized(true)
            .WithIsVisible(true)
            .WithMonths(10)
            .WithIsArchival(false)
            .Build();
            
            
        var shelters = new List<Shelter> { shelterMock };
        var users = new List<User> { userMock };
        var animals = new List<Animal> { animalMock };
        
        var sheltersMock = Mocker.MockDbSet(shelters);
        var usersMock = Mocker.MockDbSet(users);
        var animalsMock = Mocker.MockDbSet(animals);

        _dbContext = new Mock<IDataContext>();
        _dbContext.Setup(dc => dc.Users).Returns(usersMock.Object);
        _dbContext.Setup(dc => dc.Shelters).Returns(sheltersMock.Object);
        _dbContext.Setup(dc => dc.Animals).Returns(animalsMock.Object);

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
        _dbContext.Object.Users.First().RefreshToken = _authService.CreateRefreshToken();
        var credentials = new UseRefreshTokenRequest
        {
            AccessToken = _authService.CreateAccessToken(_dbContext.Object.Users.First()),
            RefreshToken = _dbContext.Object.Users.First().RefreshToken
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

        Assert.ThrowsAsync<BadRequestException>(async () => await _authService.RevokeToken(credentials, _dbContext.Object.Users.First().Id));
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
    
    [Test]
    public void GetShelterQuery_WithCorrectShelterId_ReturnsShelterDto()
    {
        // Arrange
        var shelter = _dbContext.Object.Shelters.First();
        _dbContext.Object.Users.First().ShelterId = shelter.Id;
        _dbContext.Object.Users.First().Shelter = shelter;
        var query = new GetShelterQuery(shelter.Id);
        var handler = new GetShelterQueryHandler(_dbContext.Object);
        
        // Act
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(shelter.Id));
            // Should i in assert check all fields from DTO return?
        });
    }
    
    [Test]
    public void GetShelterQuery_WithIncorrectShelterId_ThrowsBadRequestException()
    {
        // Arrange
        var query = new GetShelterQuery(Guid.NewGuid());
        var handler = new GetShelterQueryHandler(_dbContext.Object);
        
        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
    }
    
    [Test]
    public void AddPetToArchiveCommand_WithCorrectPetId_SetsIsArchivalToTrue()
    {
        // Arrange
        var animal = _dbContext.Object.Animals.First();
        var command = new AddPetToArchiveCommand(animal.Id);
        var handler = new AddPetToArchiveCommandHandler(_dbContext.Object);
        
        // Act
        handler.Handle(command, CancellationToken.None).Wait();
        
        // Assert
        Assert.That(animal.IsArchival, Is.True);
    }
    
    [Test]
    public void AddPetToArchiveCommand_WithIncorrectPetId_ThrowsBadRequestException()
    {
        // Arrange
        var command = new AddPetToArchiveCommand(Guid.NewGuid());
        var handler = new AddPetToArchiveCommandHandler(_dbContext.Object);
        
        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(command, CancellationToken.None));
    }
}