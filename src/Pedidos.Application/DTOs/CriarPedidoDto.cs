namespace Pedidos.Application.DTOs;

public class CriarPedidoDto
{
    public Guid CompradorId { get; set; }
    public List<ItemPedidoInputDto> Itens { get; set; } = new();
}
