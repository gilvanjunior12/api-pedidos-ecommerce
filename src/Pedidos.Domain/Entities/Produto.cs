using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }

    private Produto()
    {
    }

    public Produto(string nome, decimal preco, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do produto é obrigatório.");

        if (preco <= 0)
            throw new DomainException("Preço do produto deve ser maior que zero.");

        Id = id ?? Guid.NewGuid();
        Nome = nome.Trim();
        Preco = preco;
    }
}
