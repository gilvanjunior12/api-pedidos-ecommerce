using Microsoft.EntityFrameworkCore;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Persistence;

public static class DataSeeder
{
    // IDs fixos pra facilitar teste no Swagger / demo
    public static readonly Guid UsuarioDemoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ProdutoNotebookId = Guid.Parse("22222222-2222-2222-2222-222222222201");
    public static readonly Guid ProdutoMouseId = Guid.Parse("22222222-2222-2222-2222-222222222202");

    public static async Task SeedAsync(PedidosDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Usuarios.AnyAsync(cancellationToken))
            return;

        context.Usuarios.Add(new Usuario("Maria Silva", "maria@email.com", UsuarioDemoId));
        context.Produtos.Add(new Produto("Notebook", 3500.00m, ProdutoNotebookId));
        context.Produtos.Add(new Produto("Mouse", 89.90m, ProdutoMouseId));

        await context.SaveChangesAsync(cancellationToken);
    }
}
