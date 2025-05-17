using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using VotingSystem.AdminClient.Services;
using VotingSystem.AdminClient.Layout;

namespace VotingSystem.ComponentTest
{
    public class MainLayoutTests : TestContext
    {
        [Fact]
        public void MainLayout_ShouldShowLoginLink_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            var authStateMock = new Mock<IAuthState>();
            var toastServiceMock = new Mock<IToastService>();

            authStateMock.SetupGet(a => a.IsLoggedIn).Returns(false);

            Services.AddSingleton(authServiceMock.Object);
            Services.AddSingleton(authStateMock.Object);
            Services.AddSingleton(toastServiceMock.Object);
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();

            // Act
            var cut = RenderComponent<MainLayout>();

            // Assert
            var loginLink = cut.Find("a.nav-link[href='/login']");
            Assert.NotNull(loginLink);
            Assert.Equal("Login", loginLink.TextContent);
        }

        [Fact]
        public void MainLayout_ShouldShowAuthenticatedLinks_WhenUserIsLoggedIn()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            var authStateMock = new Mock<IAuthState>();
            var toastServiceMock = new Mock<IToastService>();

            authStateMock.SetupGet(a => a.IsLoggedIn).Returns(true);

            Services.AddSingleton(authServiceMock.Object);
            Services.AddSingleton(authStateMock.Object);
            Services.AddSingleton(toastServiceMock.Object);
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();

            // Act
            var cut = RenderComponent<MainLayout>();

            // Assert
            var logoutButton = cut.Find("button.btn-outline-light");
            Assert.NotNull(logoutButton);
            Assert.Equal("Logout", logoutButton.TextContent);

            var myPollsLink = cut.Find("a.nav-link[href='/mypolls']");
            Assert.NotNull(myPollsLink);
            Assert.Equal("My Polls", myPollsLink.TextContent);

            var createPollLink = cut.Find("a.nav-link[href='/create-poll']");
            Assert.NotNull(createPollLink);
            Assert.Equal("Create Poll", createPollLink.TextContent);
        }

        [Fact]
        public void MainLayout_ShouldCallLogout_WhenLogoutButtonIsClicked()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            var authStateMock = new Mock<IAuthState>();
            var toastServiceMock = new Mock<IToastService>();

            authStateMock.SetupGet(a => a.IsLoggedIn).Returns(true);
            authServiceMock.Setup(a => a.LogoutAsync()).Returns(Task.CompletedTask);

            Services.AddSingleton(authServiceMock.Object);
            Services.AddSingleton(authStateMock.Object);
            Services.AddSingleton(toastServiceMock.Object);
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();

            var cut = RenderComponent<MainLayout>();

            // Act
            var logoutButton = cut.Find("button.btn-outline-light");
            logoutButton.Click();

            // Assert
            authServiceMock.Verify(a => a.LogoutAsync(), Times.Once);

            var navigationManager = Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.Equal("http://localhost/login", navigationManager?.Uri);
        }

        [Fact]
        public void MainLayout_ShouldShowLogout_WhenUserIsLoggedIn()
        {
            // Arrange
            var authStateMock = new Mock<IAuthState>();
            authStateMock.SetupGet(a => a.IsLoggedIn).Returns(true); // Simulate logged-in user

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(service => service.GetTokenAsync()).ReturnsAsync("mock-token");

            var toastServiceMock = new Mock<IToastService>(); // Mock the IToastService

            Services.AddSingleton(authStateMock.Object);
            Services.AddSingleton(authServiceMock.Object);
            Services.AddSingleton(toastServiceMock.Object); // Register the mock IToastService

            // Act
            var cut = RenderComponent<MainLayout>();

            // Debugging: Print the rendered HTML
            Console.WriteLine(cut.Markup);

            // Assert
            var logoutButton = cut.Find("button.btn-outline-light");
            Assert.NotNull(logoutButton);
            Assert.Equal("Logout", logoutButton.TextContent);
        }
    }
}
