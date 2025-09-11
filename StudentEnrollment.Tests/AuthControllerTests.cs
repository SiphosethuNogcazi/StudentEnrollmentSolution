using Moq;
using Xunit;
using StudentEnrollment.Api.Controllers;
using StudentEnrollment.Api.Services; // where IAuthService lives
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StudentEnrollment.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsValid()
        {
            // Arrange
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
                       .ReturnsAsync(true);

            var controller = new AuthController(mockService.Object);
            var dto = new RegisterDto { Username = "test", Password = "12345" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", okResult.Value);
        }
    }
}
