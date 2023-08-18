using LapkaBackend.API.Controllers;
using LapkaBackend.Application.Functions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LapkaBackend.Application.Tests
{
    public class GetShelterByPosition
    {
        [Fact]
        public async Task GetShelterByPosition_ReturnsOkResult()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>(); // Przygotowanie mocka dla IMediator
            var controller = new ShelterController(mediatorMock.Object); // Utworzenie kontrolera

            var query = new GetShelterByPositionQuery(34,34,332,1,2); // Tworzenie zapytania

            // Konfiguracja mediatora do obs³ugi asynchronicznych zadañ
            //mediatorMock.Setup(m => m.Send(It.IsAny<GetShelterByPositionQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OkObjectResult("Test response"));

            // Act
            var result = await controller.GetShelterByPosition(query);

            // Assert
            Assert.IsType<OkObjectResult>(result); // Oczekujemy wyniku typu OkObjectResult
        }
    
    }
}