using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;

namespace Pedidos.Application.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pedido>> ListarAsync(StatusPedido? status = null, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default);
}
