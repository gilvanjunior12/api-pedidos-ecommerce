using Pedidos.Domain.Enums;

namespace Pedidos.Application.DTOs;

public class PedidoFiltroDto
{
    public StatusPedido? Status { get; set; }
}
