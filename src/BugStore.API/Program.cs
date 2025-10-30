using BugStore.API.Extensions;
using BugStore.Infra.Extensions;
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration)
    .AddDocumentationApi();

var app = builder.Build();

app.UseDocumentationApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
