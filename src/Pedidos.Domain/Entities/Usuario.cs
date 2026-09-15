using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private Usuario()
    {
    }

    public Usuario(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do usuário é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email do usuário é obrigatório.");

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Email = email.Trim();
    }
}
