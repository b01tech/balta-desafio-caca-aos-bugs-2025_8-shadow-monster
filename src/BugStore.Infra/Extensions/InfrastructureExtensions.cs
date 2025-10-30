using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using BugStore.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Infra.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            AddDataBaseContext(services, configuration);
            AddRepositories(services);
            return services;
        }

        private static void AddDataBaseContext(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<BugStoreDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql"))
            );
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICustomerReadOnlyRepository, CustomerReadOnlyRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IProductReadOnlyRepository, ProductReadOnlyRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IOrderReadOnlyRepository, OrderReadOnlyRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
