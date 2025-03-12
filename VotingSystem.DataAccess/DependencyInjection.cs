using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;

namespace VotingSystem.DataAccess {

public static class DependencyInjection {

        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {

            // Database
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<VotingSystemDbContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies()
            );

            return services;
        }
    }   
}