using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;

namespace Pedidos.UnitTests;

public class PedidoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveIniciarComoIniciado()
    {
        var pedido = CriarPedido();

        Assert.Equal(StatusPedido.Iniciado, pedido.Status);
        Assert.Single(pedido.Itens);
        Assert.True(pedido.Total > 0);
    }

    [Fact]
    public void Criar_SemItens_DeveLancarExcecao()
    {
        var comprador = new Usuario("Maria Silva", "maria@email.com");

        var ex = Assert.Throws<DomainException>(() => Pedido.Criar(comprador, Array.Empty<ItemPedido>()));

        Assert.Equal("Pedido precisa ter pelo menos um produto.", ex.Message);
    }

    [Fact]
    public void Criar_SemComprador_DeveLancarExcecao()
    {
        var produto = new Produto("Mouse", 89.90m);
        var itens = new[] { new ItemPedido(produto, 1) };

        var ex = Assert.Throws<DomainException>(() => Pedido.Criar(null!, itens));

        Assert.Equal("Pedido precisa de um comprador.", ex.Message);
    }

    [Fact]
    public void Processar_PedidoIniciado_DeveMudarParaProcessado()
    {
        var pedido = CriarPedido();

        pedido.Processar();

        Assert.Equal(StatusPedido.Processado, pedido.Status);
    }

    [Fact]
    public void Processar_PedidoJaProcessado_DeveLancarExcecao()
    {
        var pedido = CriarPedido();
        pedido.Processar();

        var ex = Assert.Throws<DomainException>(() => pedido.Processar());

        Assert.Equal("Apenas pedidos iniciados podem ser processados.", ex.Message);
    }

    [Fact]
    public void Enviar_PedidoProcessado_DeveMudarParaEnviado()
    {
        var pedido = CriarPedido();
        pedido.Processar();

        pedido.Enviar();

        Assert.Equal(StatusPedido.Enviado, pedido.Status);
    }

    [Fact]
    public void Enviar_PedidoIniciado_DeveLancarExcecao()
    {
        var pedido = CriarPedido();

        var ex = Assert.Throws<DomainException>(() => pedido.Enviar());

        Assert.Equal("Apenas pedidos processados podem ser enviados.", ex.Message);
    }

    [Fact]
    public void Cancelar_PedidoIniciado_DeveMudarParaCancelado()
    {
        var pedido = CriarPedido();

        pedido.Cancelar();

        Assert.Equal(StatusPedido.Cancelado, pedido.Status);
    }

    [Fact]
    public void Cancelar_PedidoProcessado_DeveMudarParaCancelado()
    {
        var pedido = CriarPedido();
        pedido.Processar();

        pedido.Cancelar();

        Assert.Equal(StatusPedido.Cancelado, pedido.Status);
    }

    [Fact]
    public void Cancelar_PedidoEnviado_DeveLancarExcecao()
    {
        var pedido = CriarPedido();
        pedido.Processar();
        pedido.Enviar();

        var ex = Assert.Throws<DomainException>(() => pedido.Cancelar());

        Assert.Equal("Apenas pedidos iniciados ou processados podem ser cancelados.", ex.Message);
    }

    [Fact]
    public void AtualizarItens_PedidoIniciado_DeveAtualizar()
    {
        var pedido = CriarPedido();
        var outroProduto = new Produto("Notebook", 3500m);
        var novosItens = new[] { new ItemPedido(outroProduto, 2) };

        pedido.AtualizarItens(novosItens);

        Assert.Single(pedido.Itens);
        Assert.Equal(7000m, pedido.Total);
    }

    [Fact]
    public void AtualizarItens_PedidoProcessado_DeveLancarExcecao()
    {
        var pedido = CriarPedido();
        pedido.Processar();
        var produto = new Produto("Teclado", 150m);
        var novosItens = new[] { new ItemPedido(produto, 1) };

        var ex = Assert.Throws<DomainException>(() => pedido.AtualizarItens(novosItens));

        Assert.Equal("Apenas pedidos iniciados podem ser alterados.", ex.Message);
    }

    [Fact]
    public void AtualizarItens_ListaVazia_DeveLancarExcecao()
    {
        var pedido = CriarPedido();

        var ex = Assert.Throws<DomainException>(() => pedido.AtualizarItens(Array.Empty<ItemPedido>()));

        Assert.Equal("Pedido precisa ter pelo menos um produto.", ex.Message);
    }

    private static Pedido CriarPedido()
    {
        var comprador = new Usuario("Maria Silva", "maria@email.com");
        var produto = new Produto("Mouse", 89.90m);
        return Pedido.Criar(comprador, new[] { new ItemPedido(produto, 1) });
    }
}
