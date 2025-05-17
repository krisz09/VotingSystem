using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.Models;
using Xunit;

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
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            return result.authToken;
        }

        private async Task AuthenticateAsync()
        {
            var token = await RegisterAndLoginAsync("test@example.com", "Test123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Register_ShouldSucceed()
        {
            // Arrange
            var registerData = new { Email = "newuser@example.com", Password = "Test123!" };
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users/register", registerContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);

            // Validate the response
            string authToken = result.authToken.ToString();
            string refreshToken = result.refreshToken.ToString();
            string userId = result.userId.ToString();

            authToken.Should().NotBeNullOrWhiteSpace("registration should return a valid auth token.");
            refreshToken.Should().NotBeNullOrWhiteSpace("registration should return a valid refresh token.");
            userId.Should().NotBeNullOrWhiteSpace("registration should return a valid user ID.");
        }

        [Fact]
        public async Task Register_WithExistingEmail_ShouldFail()
        {
            // Arrange
            var registerData = new { Email = "test@example.com", Password = "Test123!" }; // Email already registered
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users/register", registerContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict, "registering with an existing email should fail.");
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("User already exists");
        }

       [Fact]
        public async Task Login_ShouldSucceed()
        {
            // Arrange
            var loginData = new { Email = "test@example.com", Password = "Test123!" };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users/login", loginContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponseDto>(content);
            result.AuthToken.Should().NotBeNullOrWhiteSpace("login should return a valid auth token.");
        }

       [Fact]
        public async Task Login_WithInvalidCredentials_ShouldFail()
        {
            // Arrange
            var loginData = new { Email = "test@example.com", Password = "WrongPassword!" };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/users/login", loginContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "login with invalid credentials should fail.");
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Email or password is invalid");
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

        [Fact]
        public async Task SubmitVote_ShouldSucceed()
        {
            // Arrange
            await AuthenticateAsync();

            // Get active polls
            var activePollsResponse = await _client.GetAsync("/api/votes/active");
            activePollsResponse.EnsureSuccessStatusCode();
            var activePolls = JsonConvert.DeserializeObject<List<dynamic>>(await activePollsResponse.Content.ReadAsStringAsync());

            activePolls.Should().NotBeEmpty("there should be at least one active poll for testing.");
            var poll = activePolls[0];

            // Retrieve the user ID from the token
            var userId = "test@example.com"; // Replace this with the actual logic to retrieve the user ID if needed.

            // Submit a vote
            var voteRequest = new
            {
                PollOptionIds = new List<int> { (int)poll.pollOptions[0].id },
                UserId = userId
            };
            var voteContent = new StringContent(JsonConvert.SerializeObject(voteRequest), Encoding.UTF8, "application/json");
            var voteResponse = await _client.PostAsync("/api/votes/submit-vote", voteContent);

            // Assert
            voteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SubmitVote_WithInvalidOption_ShouldFail()
        {
            // Arrange
            await AuthenticateAsync();

            // Get active polls
            var activePollsResponse = await _client.GetAsync("/api/votes/active");
            activePollsResponse.EnsureSuccessStatusCode();
            var activePolls = JsonConvert.DeserializeObject<List<dynamic>>(await activePollsResponse.Content.ReadAsStringAsync());

            activePolls.Should().NotBeEmpty("there should be at least one active poll for testing.");
            var poll = activePolls[0];

            // Submit a vote with an invalid option ID
            var voteRequest = new
            {
                OptionIds = new List<int> { -1 } // Invalid option ID
            };
            var voteContent = new StringContent(JsonConvert.SerializeObject(voteRequest), Encoding.UTF8, "application/json");
            var voteResponse = await _client.PostAsync("/api/votes/submit-vote", voteContent);

            // Assert
            voteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "voting with an invalid option ID should not be allowed.");
        }
    }
}