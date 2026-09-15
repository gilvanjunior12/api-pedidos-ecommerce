using Pedidos.Application.DTOs;

namespace Pedidos.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoDto> CriarAsync(CriarPedidoDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PedidoDto>> ListarAsync(PedidoFiltroDto? filtro = null, CancellationToken cancellationToken = default);
    Task<PedidoDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoDto> AtualizarAsync(Guid id, AtualizarPedidoDto dto, CancellationToken cancellationToken = default);
    Task<PedidoDto> CancelarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoDto> ProcessarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PedidoDto> EnviarAsync(Guid id, CancellationToken cancellationToken = default);
}
