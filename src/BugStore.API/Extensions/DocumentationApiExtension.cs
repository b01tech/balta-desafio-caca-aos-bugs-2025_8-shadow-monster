using Microsoft.OpenApi.Models;

namespace BugStore.API.Extensions
{
    public static class DocumentationApiExtension
    {
        public static IServiceCollection AddDocumentationApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BugStore API",
                    Version = "v1",
                    Description = "API para gerenciamento",
                });
            });
            return services;
        }

        public static void UseDocumentationApi(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BugStore API v1");
                    c.RoutePrefix = "swagger";
                    c.DocumentTitle = "BugStore API Documentation";
                });
            }
        }
    }
}
