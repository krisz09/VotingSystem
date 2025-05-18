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
            if (result is null)
                throw new InvalidOperationException("Login response was null.");
            if (result.authToken is null)
                throw new InvalidOperationException("Login response did not contain an authToken.");

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

            if (result is null)
                throw new InvalidOperationException("Registration response was null.");
            if (result.authToken is null)
                throw new InvalidOperationException("Registration response did not contain an authToken.");
            if (result.refreshToken is null)
                throw new InvalidOperationException("Registration response did not contain a refreshToken.");
            if (result.userId is null)
                throw new InvalidOperationException("Registration response did not contain a userId.");

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
            if (result is null)
                throw new InvalidOperationException("Login response was null or not in the expected format.");
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

        [Fact]
        public async Task CreatePoll_ShouldReturnSuccess()
        {
            // Arrange
            await AuthenticateAsync(); // Set JWT token if needed

            var pollRequest = new
            {
                Question = "Integration test poll?",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                MinVotes = 1,
                MaxVotes = 2,
                Options = new List<string> { "Option 1", "Option 2" }
            };
            var content = new StringContent(JsonConvert.SerializeObject(pollRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/votes/create", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Integration test poll?");
        }

        [Fact]
        public async Task CreatePoll_ShouldFail_WhenInvalidDates()
        {
            await AuthenticateAsync();
            var pollRequest = new
            {
                Question = "Invalid date poll",
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(-1),
                MinVotes = 1,
                MaxVotes = 2,
                Options = new List<string> { "A", "B" }
            };
            var content = new StringContent(JsonConvert.SerializeObject(pollRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/votes/create", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("Invalid date range");
        }

        [Fact]
        public async Task CreatePoll_ShouldFail_WhenNotEnoughOptions()
        {
            await AuthenticateAsync();
            var pollRequest = new
            {
                Question = "Too few options",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                MinVotes = 1,
                MaxVotes = 2,
                Options = new List<string> { "A" } // Only one option
            };
            var content = new StringContent(JsonConvert.SerializeObject(pollRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/votes/create", content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubmitVote_ShouldFail_WhenNotAuthenticated()
        {
            var voteRequest = new
            {
                PollOptionIds = new List<int> { 1 },
                UserId = "edgecase@example.com"
            };
            var voteContent = new StringContent(JsonConvert.SerializeObject(voteRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/votes/submit-vote", voteContent);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SubmitVote_ShouldFail_WhenMissingFields()
        {
            await AuthenticateAsync();
            var voteRequest = new
            {
                PollOptionIds = new List<int>(), // Empty
                UserId = "" // Empty
            };
            var voteContent = new StringContent(JsonConvert.SerializeObject(voteRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/votes/submit-vote", voteContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var body = await response.Content.ReadAsStringAsync();
            body.Should().Contain("Invalid vote submission");
        }

        [Fact]
        public async Task SubmitVote_ShouldFail_WhenInvalidOptionId()
        {
            await AuthenticateAsync();
            var voteRequest = new
            {
                PollOptionIds = new List<int> { -1 },
                UserId = "edgecase@example.com"
            };
            var voteContent = new StringContent(JsonConvert.SerializeObject(voteRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/votes/submit-vote", voteContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetActivePolls_ShouldFail_WhenNotAuthenticated()
        {
            var response = await _client.GetAsync("/api/votes/active");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetClosedPolls_ShouldReturnEmpty_WhenNoPolls()
        {
            await AuthenticateAsync();
            var response = await _client.GetAsync("/api/votes/closed-polls");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("[]");
        }
    }

}