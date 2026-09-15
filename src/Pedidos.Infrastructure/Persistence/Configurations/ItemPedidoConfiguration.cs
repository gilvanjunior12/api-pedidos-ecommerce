using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NomeProduto).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Quantidade).IsRequired();
        builder.Property(x => x.PrecoUnitario).HasPrecision(18, 2).IsRequired();

        builder.Ignore(x => x.Subtotal);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
