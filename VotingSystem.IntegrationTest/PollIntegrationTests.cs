using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.Models;

namespace VotingSystem.IntegrationTest
{
    public class PollsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PollsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndLoginAsync(string email, string password)
        {
            // Try login
            var loginData = new { Email = email, Password = password };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/users/login", loginContent);

            if (!loginResponse.IsSuccessStatusCode)
            {
                // Register
                var registerData = new { Email = email, Password = password };
                var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");
                var registerResponse = await _client.PostAsync("/users/register", registerContent);
                registerResponse.EnsureSuccessStatusCode();

                // Retry login
                loginResponse = await _client.PostAsync("/users/login", loginContent);
            }

            loginResponse.EnsureSuccessStatusCode();
            var json = await loginResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponseDto>(json);

            return result.AuthToken;
        }

        private async Task AuthenticateAsync()
        {
            var token = await RegisterAndLoginAsync("test@example.com", "Test123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


        [Fact]
        public async Task AuthenticateAsync_ShouldSetAuthorizationHeader()
        {
            // Arrange
            var testClient = _client;

            // Act
            await AuthenticateAsync(); // this registers + logs in

            // Assert – token is now set in client headers
            testClient.DefaultRequestHeaders.Authorization
                .Should().NotBeNull("JWT token should be set after authentication");
            testClient.DefaultRequestHeaders.Authorization.Scheme
                .Should().Be("Bearer");
            testClient.DefaultRequestHeaders.Authorization.Parameter
                .Should().NotBeNullOrWhiteSpace();
        }

      



        [Fact]
        public async Task GetActivePolls_ShouldReturnSuccess()
        {
            await AuthenticateAsync();
            // Act
            var response = await _client.GetAsync("/api/votes/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Test Poll");
        }
    }

}
