using AutoMapper;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Text.Json.Serialization;
using VotingSystem.AdminClient.Services;

namespace VotingSystem.AdminClient.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBlazorServices(this IServiceCollection services, IConfiguration config)
        {
            

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new BlazorMappingProfile()));
            mapperConfig.AssertConfigurationIsValid();
            services.AddAutoMapper(typeof(BlazorMappingProfile));

            services.AddBlazoredLocalStorage();

            services.AddScoped<JsonSerializerOptions>(_ =>
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                return options;
            });

            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(config["ApiBaseUrl"]!) });
            services.AddScoped<IHttpRequestUtility, HttpRequestUtility>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IPollsService, PollsService>();
            services.AddScoped<AuthState>();

            return services;
        }
    }
}
