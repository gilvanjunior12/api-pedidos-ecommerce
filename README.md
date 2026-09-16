# API de Pedidos — E-commerce

API RESTful em **.NET 8** para gerenciamento de pedidos.

## Stack

- .NET 8 / Minimal APIs
- SQL Server 2022
- Entity Framework Core
- Docker
- Serilog

## Como rodar com Docker

1. Copie o arquivo de ambiente e defina a senha do SQL Server:

```bash
cp .env.example .env
```

2. Suba a API e o SQL Server 2022:

```bash
docker compose up --build
```

3. Acesse:

- Swagger: http://localhost:8080/swagger
- Health: http://localhost:8080/health

> Se já existir outro SQL na porta 1433, pare esse container antes do `compose up`.
