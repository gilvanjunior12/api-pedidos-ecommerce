using Pedidos.Application.DTOs;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Enums;

namespace Pedidos.Api.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/pedidos").WithTags("Pedidos");

        group.MapPost("/", CriarAsync);
        group.MapGet("/", ListarAsync);
        group.MapGet("/{id:guid}", ObterPorIdAsync);
        group.MapPut("/{id:guid}", AtualizarAsync);
        group.MapPost("/{id:guid}/cancelar", CancelarAsync);
        group.MapPost("/{id:guid}/processar", ProcessarAsync);
        group.MapPost("/{id:guid}/enviar", EnviarAsync);
    }

    private static async Task<IResult> CriarAsync(
        CriarPedidoDto dto,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.CriarAsync(dto, cancellationToken);
        return Results.Created($"/api/v1/pedidos/{pedido.Id}", pedido);
    }

    private static async Task<IResult> ListarAsync(
        StatusPedido? status,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var filtro = status.HasValue ? new PedidoFiltroDto { Status = status } : null;
        var pedidos = await service.ListarAsync(filtro, cancellationToken);
        return Results.Ok(pedidos);
    }

    private static async Task<IResult> ObterPorIdAsync(
        Guid id,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.ObterPorIdAsync(id, cancellationToken);
        return pedido is null ? Results.NotFound(new { erro = "Pedido não encontrado." }) : Results.Ok(pedido);
    }

    private static async Task<IResult> AtualizarAsync(
        Guid id,
        AtualizarPedidoDto dto,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.AtualizarAsync(id, dto, cancellationToken);
        return Results.Ok(pedido);
    }

    private static async Task<IResult> CancelarAsync(
        Guid id,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.CancelarAsync(id, cancellationToken);
        return Results.Ok(pedido);
    }

    private static async Task<IResult> ProcessarAsync(
        Guid id,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.ProcessarAsync(id, cancellationToken);
        return Results.Ok(pedido);
    }

    private static async Task<IResult> EnviarAsync(
        Guid id,
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedido = await service.EnviarAsync(id, cancellationToken);
        return Results.Ok(pedido);
    }
}
