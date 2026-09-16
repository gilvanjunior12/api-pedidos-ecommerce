using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidosDbContext _context;

    public PedidoRepository(PedidosDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Pedido>> ListarAsync(StatusPedido? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Pedidos
            .Include(p => p.Itens)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        return await query
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        await _context.Pedidos.AddAsync(pedido, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        var idsAtuais = pedido.Itens.Select(i => i.Id).ToHashSet();
        var itensNoBanco = await _context.Set<ItemPedido>()
            .Where(i => i.PedidoId == pedido.Id)
            .ToListAsync(cancellationToken);

        foreach (var antigo in itensNoBanco.Where(i => !idsAtuais.Contains(i.Id)))
            _context.Set<ItemPedido>().Remove(antigo);

        foreach (var item in pedido.Itens)
        {
            if (_context.Entry(item).State == EntityState.Detached)
                await _context.Set<ItemPedido>().AddAsync(item, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
