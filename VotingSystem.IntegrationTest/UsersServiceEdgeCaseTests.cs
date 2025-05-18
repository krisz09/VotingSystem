using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Config;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using Xunit;

namespace VotingSystem.IntegrationTest
{
    public class UsersServiceEdgeCaseIntegrationTests : IDisposable
    {
        private readonly ServiceProvider _provider;

        public UsersServiceEdgeCaseIntegrationTests()
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

        private static void SetHttpContext(IServiceProvider provider)
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = provider
            };
            httpContextAccessor.HttpContext = httpContext;
        }

        [Fact]
        public async Task Register_ShouldFail_WhenUserAlreadyExists()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();
            var email = "exists@example.com";
            var password = "Test123!";

            await usersService.RegisterAsync(email, password);

            Func<Task> act = async () => await usersService.RegisterAsync(email, password);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*User already exists*");
        }

        [Fact]
        public async Task Register_ShouldFail_WhenPasswordIsWeak()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();
            var email = "weakpass@example.com";
            var password = "123"; // Weak password

            Func<Task> act = async () => await usersService.RegisterAsync(email, password);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*User creation failed*");
        }

        [Fact]
        public async Task Login_ShouldFail_WhenUserNotFound()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();

            Func<Task> act = async () => await usersService.LoginAsync("notfound@example.com", "Test123!");
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*invalid*");
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPasswordInvalid()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();
            var email = "wrongpass@example.com";
            var password = "Test123!";

            await usersService.RegisterAsync(email, password);

            Func<Task> act = async () => await usersService.LoginAsync(email, "WrongPassword!");
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*invalid*");
        }

        [Fact]
        public async Task RedeemRefreshToken_ShouldFail_WhenTokenIsNotAGuid()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();

            Func<Task> act = async () => await usersService.RedeemRefreshTokenAsync("not-a-guid");
            await act.Should().ThrowAsync<AccessViolationException>()
                .WithMessage("*Invalid refresh token*");
        }

        [Fact]
        public async Task RedeemRefreshToken_ShouldFail_WhenUserNotFound()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();

            var invalidToken = Guid.NewGuid().ToString();
            Func<Task> act = async () => await usersService.RedeemRefreshTokenAsync(invalidToken);
            await act.Should().ThrowAsync<AccessViolationException>()
                .WithMessage("*Invalid refresh token*");
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldFail_WhenUserNotFound()
        {
            var provider = BuildServiceProvider();
            SetHttpContext(provider);
            var usersService = provider.GetRequiredService<IUsersService>();

            Func<Task> act = async () => await usersService.GetUserByIdAsync("notfound");
            await act.Should().ThrowAsync<AccessViolationException>()
                .WithMessage("*User not accessible*");
        }

        [Fact]
        public void GetCurrentUserId_ShouldReturnNull_WhenNoHttpContext()
        {
            var provider = BuildServiceProvider();
            var usersService = provider.GetRequiredService<IUsersService>();

            var userId = usersService.GetCurrentUserId();
            userId.Should().BeNull();
        }

        [Fact]
        public void GetCurrentUserRoles_ShouldReturnEmpty_WhenNoUser()
        {
            var provider = BuildServiceProvider();
            var usersService = provider.GetRequiredService<IUsersService>();

            var roles = usersService.GetCurrentUserRoles();
            roles.Should().BeEmpty();
        }
    }
}
