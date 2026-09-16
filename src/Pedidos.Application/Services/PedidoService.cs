using Pedidos.Application.DTOs;
using Pedidos.Application.Interfaces;
using Pedidos.Application.Mappings;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IProdutoRepository _produtos;

    public PedidoService(
        IPedidoRepository pedidos,
        IUsuarioRepository usuarios,
        IProdutoRepository produtos)
    {
        _pedidos = pedidos;
        _usuarios = usuarios;
        _produtos = produtos;
    }

    public async Task<PedidoDto> CriarAsync(CriarPedidoDto dto, CancellationToken cancellationToken = default)
    {
        var comprador = await _usuarios.ObterPorIdAsync(dto.CompradorId, cancellationToken)
            ?? throw new NotFoundException("Comprador não encontrado.");

        var itens = await MontarItensAsync(dto.Itens, cancellationToken);
        var pedido = Pedido.Criar(comprador, itens);

        await _pedidos.AdicionarAsync(pedido, cancellationToken);
        return PedidoMapping.ParaDto(pedido);
    }

    public async Task<IReadOnlyList<PedidoDto>> ListarAsync(PedidoFiltroDto? filtro = null, CancellationToken cancellationToken = default)
    {
        var status = filtro?.Status;
        var pedidos = await _pedidos.ListarAsync(status, cancellationToken);
        return pedidos.Select(PedidoMapping.ParaDto).ToList();
    }

    public async Task<PedidoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidos.ObterPorIdAsync(id, cancellationToken);
        return pedido is null ? null : PedidoMapping.ParaDto(pedido);
    }

    public async Task<PedidoDto> AtualizarAsync(Guid id, AtualizarPedidoDto dto, CancellationToken cancellationToken = default)
    {
        var pedido = await ObterPedidoAsync(id, cancellationToken);
        var itens = await MontarItensAsync(dto.Itens, cancellationToken);

        pedido.AtualizarItens(itens);
        await _pedidos.AtualizarAsync(pedido, cancellationToken);

        return PedidoMapping.ParaDto(pedido);
    }

    public async Task<PedidoDto> CancelarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await ObterPedidoAsync(id, cancellationToken);
        pedido.Cancelar();
        await _pedidos.AtualizarAsync(pedido, cancellationToken);
        return PedidoMapping.ParaDto(pedido);
    }

    public async Task<PedidoDto> ProcessarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await ObterPedidoAsync(id, cancellationToken);
        pedido.Processar();
        await _pedidos.AtualizarAsync(pedido, cancellationToken);
        return PedidoMapping.ParaDto(pedido);
    }

    public async Task<PedidoDto> EnviarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await ObterPedidoAsync(id, cancellationToken);
        pedido.Enviar();
        await _pedidos.AtualizarAsync(pedido, cancellationToken);
        return PedidoMapping.ParaDto(pedido);
    }

    private async Task<Pedido> ObterPedidoAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _pedidos.ObterPorIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Pedido não encontrado.");
    }

    private async Task<List<ItemPedido>> MontarItensAsync(List<ItemPedidoInputDto> itensDto, CancellationToken cancellationToken)
    {
        if (itensDto is null || itensDto.Count == 0)
            throw new DomainException("Pedido precisa ter pelo menos um produto.");

        var ids = itensDto.Select(i => i.ProdutoId).Distinct().ToList();
        var produtos = await _produtos.ObterPorIdsAsync(ids, cancellationToken);
        var mapa = produtos.ToDictionary(p => p.Id);

        var itens = new List<ItemPedido>();
        foreach (var itemDto in itensDto)
        {
            if (!mapa.TryGetValue(itemDto.ProdutoId, out var produto))
                throw new NotFoundException($"Produto {itemDto.ProdutoId} não encontrado.");

            itens.Add(new ItemPedido(produto, itemDto.Quantidade));
        }

        return itens;
    }
}
