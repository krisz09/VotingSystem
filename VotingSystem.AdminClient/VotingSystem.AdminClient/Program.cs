using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VotingSystem.AdminClient.Infrastructure;

namespace VotingSystem.AdminClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazorServices(builder.Configuration);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7294") });

            await builder.Build().RunAsync();
        }
    }
}
