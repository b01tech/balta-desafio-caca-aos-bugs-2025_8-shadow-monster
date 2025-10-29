using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Infra.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDataBaseContext(services, configuration);
            return services;
        }

        private static void AddDataBaseContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BugStoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));
        }
    }
}
