using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pedidos.Api.Endpoints;
using Pedidos.Api.Exceptions;
using Pedidos.Infrastructure;
using Pedidos.Infrastructure.Persistence;

CarregarEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PedidosDbContext>("database");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pedidos API",
        Version = "v1",
        Description = "API REST de pedidos de e-commerce"
    });
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API v1");
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

app.MapHealthChecks("/health");
app.MapPedidoEndpoints();

app.Run();

static void CarregarEnv()
{
    var candidatos = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env")),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env"))
    };

    foreach (var caminho in candidatos.Distinct())
    {
        if (File.Exists(caminho))
        {
            DotNetEnv.Env.Load(caminho);
            break;
        }
    }
}
