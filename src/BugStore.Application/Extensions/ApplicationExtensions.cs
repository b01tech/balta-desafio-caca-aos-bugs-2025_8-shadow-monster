using BugStore.Application.Services.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Application.Extensions;
public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ConfigureMediator(services);
        AddServices(services);
        return services;
    }
    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IMediatorService, MediatorService>();
    }
    private static void ConfigureMediator(IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Namespace = "BugStore.Application";
        });
    }

}
