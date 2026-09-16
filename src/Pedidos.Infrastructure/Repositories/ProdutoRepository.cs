using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly PedidosDbContext _context;

    public ProdutoRepository(PedidosDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> ObterPorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var lista = ids.Distinct().ToList();
        return await _context.Produtos
            .Where(p => lista.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}
