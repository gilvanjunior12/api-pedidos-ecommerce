using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }

    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemPedido()
    {
    }

    public ItemPedido(Produto produto, int quantidade)
    {
        if (produto is null)
            throw new DomainException("Produto do item é obrigatório.");

        if (quantidade <= 0)
            throw new DomainException("Quantidade do item deve ser maior que zero.");

        Id = Guid.NewGuid();
        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        Quantidade = quantidade;
        PrecoUnitario = produto.Preco;
    }

    internal void VincularPedido(Guid pedidoId)
    {
        PedidoId = pedidoId;
    }
}
