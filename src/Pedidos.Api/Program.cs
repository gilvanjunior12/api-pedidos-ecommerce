using Microsoft.EntityFrameworkCore;
using Pedidos.Infrastructure;
using Pedidos.Infrastructure.Persistence;

CarregarEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

app.MapGet("/", () => "Hello World!");

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
