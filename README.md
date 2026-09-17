# API de Pedidos — E-commerce

API RESTful em **.NET 8** para gerenciamento de pedidos de e-commerce.

## Stack

- .NET 8 / Minimal APIs
- SQL Server 2022
- Entity Framework Core
- Clean Architecture
- Serilog (console + arquivo)
- Docker / Docker Compose
- Swagger + health check
- Testes unitários (xUnit)

## Pré-requisitos

- .NET 8 SDK
- Docker Desktop (para Compose ou só o SQL)
- Arquivo `.env` na raiz (copie de `.env.example` e ajuste a senha)

```bash
cp .env.example .env
```

---

## Como rodar com Docker (Recomendado)

Sobe a API e o SQL Server juntos:

```bash
docker compose up --build
```

- Swagger: http://localhost:8080/swagger
- Health: http://localhost:8080/health

> Se já existir outro SQL na porta `1433`, pare esse container antes.

Para encerrar:

```bash
docker compose down
```

---

## Como rodar sem Docker (API local)

A API sobe com `dotnet run`. Você ainda precisa de um **SQL Server 2022** em `localhost,1433` (instalação local ou só o container do SQL).

1. Configure o `.env` na raiz do repositório (connection string apontando para `localhost,1433`).
2. Na pasta do projeto:

```bash
dotnet restore
dotnet run --project src/Pedidos.Api
```

3. Acesse:

- Swagger: http://localhost:5141/swagger
- Health: http://localhost:5141/health

Migration e seed rodam na subida da API.

Logs em Development: `C:\Aplicativos\LogsDesafio\`.

---

## Dados de seed (teste no Swagger)

Na primeira execução a API grava:

| Recurso | Id |
|---------|-----|
| Usuário Maria Silva | `11111111-1111-1111-1111-111111111111` |
| Produto Notebook | `22222222-2222-2222-2222-222222222201` |
| Produto Mouse | `22222222-2222-2222-2222-222222222202` |

Exemplo de body para criar pedido:

```json
{
  "compradorId": "11111111-1111-1111-1111-111111111111",
  "itens": [
    {
      "produtoId": "22222222-2222-2222-2222-222222222202",
      "quantidade": 1
    }
  ]
}
```

---

## Endpoints

Base: `/api/v1/pedidos`

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v1/pedidos` | Criar pedido |
| GET | `/api/v1/pedidos` | Listar (`?status=` opcional) |
| GET | `/api/v1/pedidos/{id}` | Buscar por id |
| PUT | `/api/v1/pedidos/{id}` | Alterar itens (só Iniciado) |
| POST | `/api/v1/pedidos/{id}/cancelar` | Cancelar (status, não apaga) |
| POST | `/api/v1/pedidos/{id}/processar` | Iniciado → Processado |
| POST | `/api/v1/pedidos/{id}/enviar` | Processado → Enviado |
| GET | `/health` | Health check |

---

## Testes

```bash
dotnet test
```

Foco nas regras de status e validações do domínio.
