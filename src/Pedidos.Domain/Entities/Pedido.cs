using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.Entities;

public class Pedido
{
    private readonly List<ItemPedido> _itens = new();

    public Guid Id { get; private set; }
    public Guid CompradorId { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public decimal Total => _itens.Sum(i => i.Subtotal);

    private Pedido()
    {
    }

    public static Pedido Criar(Usuario comprador, IEnumerable<ItemPedido> itens)
    {
        if (comprador is null)
            throw new DomainException("Pedido precisa de um comprador.");

        var lista = itens?.ToList() ?? new List<ItemPedido>();
        if (lista.Count == 0)
            throw new DomainException("Pedido precisa ter pelo menos um produto.");

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            CompradorId = comprador.Id,
            Status = StatusPedido.Iniciado,
            CriadoEm = DateTime.UtcNow
        };

        foreach (var item in lista)
        {
            item.VincularPedido(pedido.Id);
            pedido._itens.Add(item);
        }

        return pedido;
    }

    public bool PodeSerAlterado() => Status == StatusPedido.Iniciado;

    public void AtualizarItens(IEnumerable<ItemPedido> novosItens)
    {
        if (!PodeSerAlterado())
            throw new DomainException("Apenas pedidos iniciados podem ser alterados.");

        var lista = novosItens?.ToList() ?? new List<ItemPedido>();
        if (lista.Count == 0)
            throw new DomainException("Pedido precisa ter pelo menos um produto.");

        _itens.Clear();
        foreach (var item in lista)
        {
            item.VincularPedido(Id);
            _itens.Add(item);
        }
    }

    public void Processar()
    {
        if (Status != StatusPedido.Iniciado)
            throw new DomainException("Apenas pedidos iniciados podem ser processados.");

        Status = StatusPedido.Processado;
    }

    public void Enviar()
    {
        if (Status != StatusPedido.Processado)
            throw new DomainException("Apenas pedidos processados podem ser enviados.");

        Status = StatusPedido.Enviado;
    }

    public void Cancelar()
    {
        if (Status is not (StatusPedido.Iniciado or StatusPedido.Processado))
            throw new DomainException("Apenas pedidos iniciados ou processados podem ser cancelados.");

        Status = StatusPedido.Cancelado;
    }
}
