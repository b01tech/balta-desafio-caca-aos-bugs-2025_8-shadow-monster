using System.Text.Json;

namespace BugStore.API.Extensions
{
    public static class ControllerExtensions
    {
        public static IServiceCollection AddControllerOptions(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new BugStore.API.Extensions.DateTimeConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            return services;
        }
    }
}
