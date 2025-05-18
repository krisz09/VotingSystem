using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VotingSystem.DataAccess.Services;

namespace VotingSystem.DataAccess.Extensions
{

    public static class DependencyInjection
    {

        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {

            // Database
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<VotingSystemDbContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
            );

            services.AddScoped<IPollsService, PollsService>();
            services.AddScoped<IUsersService, UsersService>();



            return services;
        }

        public static void ConfigureDatabaseAndIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            // Database setup
            services.AddDbContext<VotingSystemDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity setup
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<VotingSystemDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secret = configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT secret is not configured (Jwt:Secret).");

            var key = Encoding.UTF8.GetBytes(secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT ERROR: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT OK: Token is valid.");
                        return Task.CompletedTask;
                    }
                };


            });
        }
    }
}