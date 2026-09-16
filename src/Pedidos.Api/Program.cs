using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

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
