namespace Pedidos.Application.DTOs;

public class AtualizarPedidoDto
{
    public List<ItemPedidoInputDto> Itens { get; set; } = new();
}
