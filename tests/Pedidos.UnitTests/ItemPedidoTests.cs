using Pedidos.Domain.Entities;
using Pedidos.Domain.Exceptions;

namespace Pedidos.UnitTests;

public class ItemPedidoTests
{
    [Fact]
    public void Criar_QuantidadeZero_DeveLancarExcecao()
    {
        var produto = new Produto("Mouse", 89.90m);

        var ex = Assert.Throws<DomainException>(() => new ItemPedido(produto, 0));

        Assert.Equal("Quantidade do item deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Criar_ComDadosValidos_DeveCalcularSubtotal()
    {
        var produto = new Produto("Notebook", 3500m);

        var item = new ItemPedido(produto, 2);

        Assert.Equal(7000m, item.Subtotal);
        Assert.Equal(produto.Id, item.ProdutoId);
    }
}
