using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Extensions;

namespace VotingSystem.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDataAccess(builder.Configuration);
            builder.Services.ConfigureDatabaseAndIdentity(builder.Configuration);
            builder.Services.ConfigureJwtAuthentication(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                Console.WriteLine("Seeding...");
                var context = scope.ServiceProvider.GetRequiredService<VotingSystemDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                await context.Database.MigrateAsync(); // vagy EnsureCreatedAsync()

                await context.SeedAsync(userManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
