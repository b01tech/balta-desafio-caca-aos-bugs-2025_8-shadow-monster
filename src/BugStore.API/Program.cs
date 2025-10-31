using BugStore.API.Extensions;
using BugStore.API.Filter;
using BugStore.Application.Extensions;
using BugStore.Infra.Extensions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerOptions();
builder.Services.AddMvc(opt => opt.Filters.Add(typeof(GlobalExceptionFilter)));
builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
builder.Services.AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddCorsPolicy()
    .AddDocumentationApi();

var app = builder.Build();

app.UseDocumentationApi();
app.UseCorsPolicy();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.Services.ApplyPendingMigrationsAsync();

app.Run();
