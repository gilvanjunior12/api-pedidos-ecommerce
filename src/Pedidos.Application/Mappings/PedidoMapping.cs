using Pedidos.Application.DTOs;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Mappings;

public static class PedidoMapping
{
    public static PedidoDto ParaDto(Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            CompradorId = pedido.CompradorId,
            Status = pedido.Status,
            CriadoEm = pedido.CriadoEm,
            Total = pedido.Total,
            Itens = pedido.Itens.Select(i => new ItemPedidoDto
            {
                ProdutoId = i.ProdutoId,
                NomeProduto = i.NomeProduto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
