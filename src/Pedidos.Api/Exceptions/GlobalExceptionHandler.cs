using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, mensagem) = exception switch
        {
            NotFoundException e => (HttpStatusCode.NotFound, e.Message),
            DomainException e => (HttpStatusCode.BadRequest, e.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno no servidor.")
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado na API");
        else
            _logger.LogWarning(exception, "Falha de negócio: {Mensagem}", mensagem);

        httpContext.Response.StatusCode = (int)status;
        await httpContext.Response.WriteAsJsonAsync(new { erro = mensagem }, cancellationToken);
        return true;
    }
}
