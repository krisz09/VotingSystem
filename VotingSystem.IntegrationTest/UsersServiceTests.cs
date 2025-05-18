using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Config;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using Xunit;

namespace VotingSystem.IntegrationTest
{
    public class UsersServiceIntegrationTests : IDisposable
    {
        private readonly ServiceProvider _provider;

        public UsersServiceIntegrationTests()
        {
            _provider = BuildServiceProvider();
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // Use a unique DB name for each test
            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<VotingSystemDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddLogging(builder => builder.AddConsole());

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<VotingSystemDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<JwtSettings>(opts =>
            {
                opts.Secret = "supersecretkeysupersecretkey123456";
                opts.Audience = "testaudience";
                opts.Issuer = "testissuer";
                opts.AccessTokenExpirationMinutes = 60;
            });

            services.AddScoped<IUsersService, UsersService>();

            var provider = services.BuildServiceProvider();

            // Seed the Admin role
            using (var scope = provider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                }
            }
            return provider;
        }


        public void Dispose()
        {
            _provider.Dispose();
        }

        [Fact]
        public async Task RegisterAndLogin_ShouldSucceed()
        {
            // Arrange
            var provider = BuildServiceProvider();
            var db = provider.GetRequiredService<VotingSystemDbContext>();
            var usersService = provider.GetRequiredService<IUsersService>();
            var logger = provider.GetRequiredService<ILogger<UsersServiceIntegrationTests>>();

            // Set a dummy HttpContext with RequestServices
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = provider
            };
            httpContextAccessor.HttpContext = httpContext;

            var email = "integrationtest@example.com";
            var password = "Test123!";

            logger.LogInformation("Starting registration for {Email}", email);

            // Act: Register
            var (authToken, refreshToken, userId) = await usersService.RegisterAsync(email, password);

            logger.LogInformation("Registration succeeded. AuthToken: {AuthToken}, RefreshToken: {RefreshToken}, UserId: {UserId}", authToken, refreshToken, userId);

            // Assert: Registration
            authToken.Should().NotBeNullOrWhiteSpace();
            refreshToken.Should().NotBeNullOrWhiteSpace();
            userId.Should().NotBeNullOrWhiteSpace();

            logger.LogInformation("Starting login for {Email}", email);

            // Act: Login
            var (loginToken, loginRefresh, loginUserId) = await usersService.LoginAsync(email, password);

            logger.LogInformation("Login succeeded. AuthToken: {AuthToken}, RefreshToken: {RefreshToken}, UserId: {UserId}", loginToken, loginRefresh, loginUserId);

            // Assert: Login
            loginToken.Should().NotBeNullOrWhiteSpace();
            loginRefresh.Should().NotBeNullOrWhiteSpace();
            loginUserId.Should().Be(userId);
        }



        [Fact]
        public async Task Register_ShouldFail_WhenUserExists()
        {
            // Arrange
            var provider = BuildServiceProvider();
            var usersService = provider.GetRequiredService<IUsersService>();
            var email = "duplicate@example.com";
            var password = "Test123!";

            // Register first time
            await usersService.RegisterAsync(email, password);

            // Act & Assert: Register again should throw
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                usersService.RegisterAsync(email, password));
        }

        [Fact]
        public async Task Login_ShouldFail_WithWrongPassword()
        {
            // Arrange
            var provider = BuildServiceProvider();
            var usersService = provider.GetRequiredService<IUsersService>();
            var email = "wrongpass@example.com";
            var password = "Test123!";

            await usersService.RegisterAsync(email, password);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                usersService.LoginAsync(email, "WrongPassword!"));
        }
    }
}
