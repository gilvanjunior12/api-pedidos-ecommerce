using Pedidos.Domain.Enums;

namespace Pedidos.Application.DTOs;

public class PedidoDto
{
    public Guid Id { get; set; }
    public Guid CompradorId { get; set; }
    public StatusPedido Status { get; set; }
    public DateTime CriadoEm { get; set; }
    public decimal Total { get; set; }
    public List<ItemPedidoDto> Itens { get; set; } = new();
}
