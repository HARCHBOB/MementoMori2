using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MementoMori.API.Controllers;
using MementoMori.API.Interfaces;
using MementoMori.API.DTOS;
using MementoMori.API.Exceptions;

namespace MementoMori.Tests.UnitTests.ControllerTests
{
    public class ShopControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IAuthRepo> _mockAuthRepo;
        private readonly ShopController _controller;

        public ShopControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockAuthRepo = new Mock<IAuthRepo>();

            _controller = new ShopController(_mockAuthService.Object, _mockAuthRepo.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task UpdateCardColor_ReturnsUnauthorized_WhenUserIdIsNull()
        {
            var request = new UpdateColorRequest { NewColor = "Blue" };

            _mockAuthService
                .Setup(auth => auth.GetRequesterId(It.IsAny<HttpContext>()))
                .Returns((Guid?)null);
            var result = await _controller.UpdateCardColor(request);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UpdateCardColor_ReturnsOk_WhenUpdateSucceeds()
        {
            var userId = Guid.NewGuid();
            var request = new UpdateColorRequest { NewColor = "Red" };

            _mockAuthService
                .Setup(auth => auth.GetRequesterId(It.IsAny<HttpContext>()))
                .Returns(userId);

            _mockAuthRepo
                .Setup(repo => repo.UpdateUserCardColor(userId, request.NewColor))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateCardColor(request);

            Assert.IsType<OkResult>(result);
        }
    }
}
