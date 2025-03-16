using MementoMori.API.Controllers;
using MementoMori.API.DTOS;
using MementoMori.API.Exceptions;
using MementoMori.API.Interfaces;
using MementoMori.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MementoMori.Tests.UnitTests.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IAuthRepo> _mockAuthRepo;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {

            _mockAuthService = new Mock<IAuthService>();
            _mockAuthRepo = new Mock<IAuthRepo>();

            _controller = new AuthController(_mockAuthService.Object, _mockAuthRepo.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            var registerDetails = new RegisterDetails
            {
                Username = "testUser",
                Password = "TestPassword123!",
                RememberMe = true
            };
            var userId = Guid.NewGuid();
            _mockAuthRepo
                .Setup(service => service.CreateUserAsync(registerDetails))
                .ReturnsAsync(new User { Id = userId, Username = "Username", Password = "Password", CardColor = "white" });
            _mockAuthService
                .Setup(service => service.AddCookie(It.IsAny<HttpContext>(), userId, registerDetails.RememberMe));

            var result = await _controller.Register(registerDetails);

            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            var loginDetails = new LoginDetails
            {
                Username = "testUser",
                Password = "TestPassword123!",
                RememberMe = true
            };
            var userId = Guid.NewGuid();
            var mockUser = new User { Id = userId, Username = "Username", Password = "hashedPassword", CardColor = "white" };
            _mockAuthRepo
                .Setup(service => service.GetUserByUsernameAsync(loginDetails.Username))
                .ReturnsAsync(mockUser);
            _mockAuthService
                .Setup(service => service.VerifyPassword(loginDetails.Password, mockUser.Password))
                .Returns(true);
            _mockAuthService
                .Setup(service => service.AddCookie(It.IsAny<HttpContext>(), userId, loginDetails.RememberMe));

            var result = await _controller.Login(loginDetails);

            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            var loginDetails = new LoginDetails
            {
                Username = "nonExistentUser",
                Password = "TestPassword123!"
            };
            _mockAuthRepo
                .Setup(service => service.GetUserByUsernameAsync(loginDetails.Username))
                .ThrowsAsync(new UserNotFoundException());

            var result = await _controller.Login(loginDetails);

            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            var loginDetails = new LoginDetails
            {
                Username = "testUser",
                Password = "WrongPassword!"
            };
            var mockUser = new User { Id = Guid.NewGuid(), Username = "Username", Password = "hashedPassword", CardColor = "white" };
            _mockAuthRepo
                .Setup(service => service.GetUserByUsernameAsync(loginDetails.Username))
                .ReturnsAsync(mockUser);
            _mockAuthService
                .Setup(service => service.VerifyPassword(loginDetails.Password, mockUser.Password))
                .Returns(false);

            var result = await _controller.Login(loginDetails);

            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
