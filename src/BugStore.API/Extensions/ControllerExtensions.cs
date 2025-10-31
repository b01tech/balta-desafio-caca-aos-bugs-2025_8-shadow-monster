using System.Text.Json;

namespace BugStore.API.Extensions
{
    public static class ControllerExtensions
    {
        public static IServiceCollection AddControllerOptions(this IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new BugStore.API.Extensions.DateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new BugStore.API.Extensions.GuidConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            return services;
        }
    }
}
