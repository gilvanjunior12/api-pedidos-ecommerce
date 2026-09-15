using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.CriadoEm).IsRequired();

        builder.Ignore(x => x.Total);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.CompradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Itens)
            .WithOne()
            .HasForeignKey(x => x.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
