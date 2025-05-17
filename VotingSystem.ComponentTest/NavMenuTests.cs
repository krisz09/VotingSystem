using Bunit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using VotingSystem.AdminClient.Layout;
using FluentAssertions;
using VotingSystem.AdminClient.Services;

namespace VotingSystem.ComponentTest
{
    public class NavMenuTests : TestContext
    {
        
        [Fact]
        public void NavMenu_ToggleNavMenu_ShouldCollapseAndExpand()
        {
            // Arrange
            var cut = RenderComponent<NavMenu>();

            // Act
            var toggleButton = cut.Find("div.nav-scrollable");
            toggleButton.Click(); // Simulate click to expand
            toggleButton.Click(); // Simulate click to collapse

            // Assert
            Assert.Contains("collapse", cut.Find("div.nav-scrollable").ClassList);
        }

        [Fact]
        public void NavMenu_ShouldNotShowLogout_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var authState = new AuthenticationState(new System.Security.Claims.ClaimsPrincipal()); // No identity
            var authStateProviderMock = new Mock<AuthenticationStateProvider>();
            authStateProviderMock.Setup(provider => provider.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            Services.AddSingleton<AuthenticationStateProvider>(authStateProviderMock.Object);

            var cut = RenderComponent<NavMenu>();

            // Act
            var logoutLink = cut.FindAll("a").FirstOrDefault(a => a.TextContent.Contains("Logout"));

            // Assert
            Assert.Null(logoutLink);
        }
    }
}