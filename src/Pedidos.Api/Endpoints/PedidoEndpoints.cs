using Pedidos.Application.DTOs;
using Pedidos.Application.Interfaces;

namespace Pedidos.Api.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pedidos");

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
        return Results.Created($"/pedidos/{pedido.Id}", pedido);
    }

    private static async Task<IResult> ListarAsync(
        IPedidoService service,
        CancellationToken cancellationToken)
    {
        var pedidos = await service.ListarAsync(cancellationToken: cancellationToken);
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
