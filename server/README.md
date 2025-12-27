# 🏢 Server — API .NET 8 (SaaS Multi-Tenant)

Backend da plataforma **Locadora de Automóveis**, implementando **Clean Architecture**, **CQRS (MediatR)** e **DDD**, com **multi-tenancy** por `TenantId` e isolamento via **Global Query Filters** (EF Core).

> Para visão geral do produto (incluindo Web Admin e Web Portal), veja o README da raiz.

---

## ✅ Destaques (confirmados no código)

### 🔐 Multi-tenancy seguro
- Isolamento lógico por tenant via **Global Query Filters** no EF Core.
- Entidades multi-tenant carregam `TenantId` e o `DbContext` aplica o filtro automaticamente.
- Suporte a **tenant override por header** (`X-Tenant-Override`) via `TenantOverrideMiddleware`, restrito a **PlatformAdmin** e com **log de segurança**.

### 🧠 CQRS + MediatR com controllers finos
- Controllers fazem orquestração mínima e delegam para handlers via `MediatR.Send(...)`.
- Existem **82 endpoints/handlers** separados por caso de uso (leitura/escrita), mantendo regra de negócio fora da borda HTTP.

Distribuição (alto nível):
- `auth` (11), `rental` (10), `coupon` (7), `vehicle` (7)
- `client` (6), `employee` (6), `group` (6), `rental-extras` (6)
- `billing-plan` (5), `driver` (5), `partner` (5)
- `account` (3), `admin` (2), `configuration` (2), `search` (1)

### ♻️ Autenticação resiliente (JWT + Refresh Token)
- JWT Bearer + Refresh Token com **rotação**, persistência em **hash** e encadeamento (`ReplacedByTokenHash`).
- Refresh token em cookie com `HttpOnly`, `Secure`, `SameSite=None` e escopo por path.
- **Hangfire** com job recorrente para limpeza de tokens expirados/revogados.

### 💰 Motor de precificação no Domínio
Cálculo de locação encapsulado no domínio (`RentalCalculator`), considerando:
- Planos (Diário / Controlado / Livre)
- Extras por dia
- Penalidades (atraso, combustível)
- Aplicação de cupons
- Quilometragem (quando aplicável)
- Resultado contratual (`RentalFinalized`) pronto pra UI/relatórios

### ✅ Result Pattern (FluentResults)
- Erros previsíveis tratados com `Result/Result<T>` (sem exceção como controle de fluxo).
- Exceções inesperadas: handler global + rollback (quando aplicável).

---

## 📂 Estrutura do projeto

```text
server/
├── core/
│   ├── domain/            # Regras de negócio (entidades, VOs, domain services)
│   └── application/       # Casos de uso (handlers CQRS, DTOs, validações)
│
├── infrastructure/
│   ├── orm/               # EF Core (DbContext, mappings, repos)
│   └── s3/                # Integração S3 compatível (Cloudflare R2 via AWS SDK)
│
├── web-api/               # Controllers, DI, middlewares, Identity, Jobs, Swagger
│
└── tests/
    ├── unit/              # MSTest + Moq (domínio, handlers, validações)
    └── integration/       # Integração (infra + API/repositórios)
````

---

## ⚡ Rodar localmente (dev)

### Pré-requisitos

* .NET SDK 8+
* SQL Server (local ou container)
* Redis
* (para testes de integração) Docker/Podman disponível

### Subir API

```bash
cd server/web-api
dotnet restore
dotnet run
```

> Swagger: `https://localhost:5055/swagger`

---

## ⚙️ Configuração (IConfiguration)

A API usa:

1. **Chaves avulsas**: `builder.Configuration["SQL_CONNECTION_STRING"]`, etc.
2. **Options Pattern** (seções tipadas): `services.Configure<T>(GetSection("..."))`

> Em env vars (Docker/CI), use `__` no lugar de `:` em chaves aninhadas
> Ex.: `MAILOPTIONS__HOST`, `APPURLS__ADMINAPP`.

### Setup rápido com user-secrets (dev)

```bash
cd server/web-api
dotnet user-secrets init

dotnet user-secrets set "SQL_CONNECTION_STRING" "<...>"
dotnet user-secrets set "JWT_GENERATION_KEY" "<...>"
dotnet user-secrets set "JWT_AUDIENCE_DOMAIN" "<...>"
```

### Variáveis/config keys (principais)

**Banco / Jobs / Cache**

* `SQL_CONNECTION_STRING`
* `HANGFIRE_SQL_CONNECTION_STRING`
* `REDIS_CONNECTION_STRING`

**JWT / Auth**

* `JWT_GENERATION_KEY`
* `JWT_AUDIENCE_DOMAIN`
* `GOOGLE_CLIENT_ID` (quando login Google é usado no front)
* `LUCKYPENNYSOFTWARE_LICENSE_KEY`

**Seções (Options Pattern)**

* `APPURLS` (`ADMINAPP`, `PORTALAPP`)
* `MAILOPTIONS` (SMTP)
* `CLOUDFLARE_R2_CREDENTIALS` (R2/S3 compatível)
* `RefreshTokenOptions`

**Outros**

* `CORS_ALLOWED_ORIGINS` (separadas por `;`)
* `CAPTCHA_KEY`
* `CAPTCHA_ADMIN` (bypass PlatformAdmin — use com cuidado)
* `NEWRELIC_LICENSE_KEY`

**Seed / Bootstrap**

* `PLATFORM_ADMIN_FULLNAME`
* `PLATFORM_ADMIN_USERNAME`
* `PLATFORM_ADMIN_EMAIL`
* `PLATFORM_ADMIN_PASSWORD`

> Obs.: a tabela completa e padronizada (IConfiguration ↔ Env var) está no README da raiz.

---

## 🧪 Testes

Rodar testes:

```bash
dotnet test server/tests/unit/LocadoraDeAutomoveis.Tests.Unit.csproj
dotnet test server/tests/integration/LocadoraDeAutomoveis.Tests.Integration.csproj
```

---

## 🔎 Troubleshooting rápido

* **401/403 inesperado**: confira `JWT_AUDIENCE_DOMAIN`, clock/expiração e cookies cross-site (SameSite/HTTPS).
* **Vazamento de tenant**: verifique se `TenantId` está sendo setado na criação e se o tenant resolver está funcionando antes do DbContext.
* **Jobs não rodam**: valide `HANGFIRE_SQL_CONNECTION_STRING` e se o server está processando workers.

---

**Gustavo Santos**
*Full Stack Developer .NET & Angular*