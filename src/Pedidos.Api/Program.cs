using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pedidos.Api.Endpoints;
using Pedidos.Api.Exceptions;
using Pedidos.Api.Swagger;
using Pedidos.Infrastructure;
using Pedidos.Infrastructure.Persistence;
using Serilog;
using Serilog.Events;

CarregarEnv();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

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
        options.SchemaFilter<EnumSchemaFilter>();
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, _, exception) =>
        {
            var path = httpContext.Request.Path.Value ?? string.Empty;

            // Swagger, health e estáticos: não poluem o arquivo em Information
            if (EhRuidoDeRequest(path))
                return LogEventLevel.Verbose;

            if (exception is not null || httpContext.Response.StatusCode >= 500)
                return LogEventLevel.Error;

            if (httpContext.Response.StatusCode >= 400)
                return LogEventLevel.Warning;

            return LogEventLevel.Information;
        };
    });
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

    Log.Information("API de pedidos iniciada");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao iniciar a API");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static bool EhRuidoDeRequest(string path)
{
    if (string.IsNullOrEmpty(path))
        return false;

    if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
        || path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
        return true;

    return path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith(".map", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
}

static void CarregarEnv()
{
    // Procura .env na raiz do repo (funciona rodando pela Api ou pela solution)
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
