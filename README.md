# 🚗 Locadora de Automóveis (Plataforma SaaS Multi-Tenant)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-20-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Tests](https://img.shields.io/badge/Testcontainers-Available-green?style=for-the-badge)

Plataforma completa de gestão de aluguel de veículos desenvolvida como case avançado de **Clean Architecture**, **CQRS** e **DDD**.
O sistema opera como **SaaS Multi-Tenant híbrido**, suportando múltiplas locadoras isoladas logicamente (por `TenantId`), com dois front-ends independentes:

- **Web Admin**: painel de gestão (operações internas / plataforma)
- **Web Portal**: autoatendimento do cliente (self-service)

---

## ⚡ Quickstart (rodar localmente)

### Pré-requisitos

- .NET SDK 8+
- Node.js + npm (compatível com Angular CLI)
- SQL Server (local ou container)
- Redis

### 1) Backend (API)

O projeto foi desenhado para simular ambiente de produção, então integrações (email, storage, jobs, observabilidade) fazem parte do runtime.

- Configure as variáveis do Backend (**dev:** `dotnet user-secrets` / **docker-ci:** env vars). [⚙️ Configuração (variáveis de ambiente)](#config)

```bash
cd server/web-api
dotnet user-secrets init
dotnet user-secrets set "SQL_CONNECTION_STRING" "<...>"
dotnet user-secrets set "JWT_GENERATION_KEY" "<...>"
dotnet user-secrets set "JWT_AUDIENCE_DOMAIN" "<...>"
dotnet restore
dotnet run
```

> API disponível em `https://localhost:5055/swagger`

### 2) Web Admin

```bash
cd web-admin
npm install
npm start
```

### 3) Web Portal

```bash
cd web-portal
npm install
npm start
```

---

## 🚀 Destaques Arquiteturais & Features

Este projeto vai além de CRUD: ele implementa soluções típicas de sistemas corporativos (multi-tenant, sessão resiliente, domínio rico e operações assíncronas).

### 🏢 Backend (.NET 8)

#### ✅ Multi-tenancy seguro (por TenantId)

- Isolamento lógico de dados com **Global Query Filters** no EF Core.
- Todas as entidades multi-tenant usam `TenantId` e o `DbContext` aplica filtro automático para evitar vazamento entre tenants.
- Há suporte a **tenant override por header** via `TenantOverrideMiddleware`, restrito a administradores da plataforma (**role `PlatformAdmin`**) e com **log de segurança**.

#### ✅ CQRS + MediatR com Controllers finos

- Controllers fazem orquestração mínima e delegam para handlers via `MediatR.Send(...)`.
- Existem **82 handlers** separando casos de uso de leitura e escrita e mantendo a lógica de negócio longe da borda HTTP.

#### ✅ Autenticação resiliente (JWT + Refresh Token)

- JWT Bearer + Refresh Token com **rotação**, persistência em **hash** e substituição encadeada (ex.: `ReplacedByTokenHash`).
- Refresh Token entregue via cookie com `HttpOnly`, `Secure`, `SameSite=None` e escopo de path.
- Job Hangfire recorrente para **limpeza de refresh tokens** expirados/revogados.

#### ✅ Motor de precificação no Domínio

O cálculo de locação fica encapsulado no domínio (`RentalCalculator`) e considera:

- Planos variados (Diário, Controlado, Livre)
- Extras por dia
- Penalidades (atraso, nível de combustível)
- Aplicação de cupons
- Quilometragem rodada (quando aplicável)
- Retorno de um "contrato" (`RentalFinalized`) pronto para ser usado por UI/relatórios

#### ✅ Result Pattern (FluentResults)

- Fluxos de erro previsíveis são tratados por `FluentResults`, evitando exceções como "controle de fluxo".
- Exceções inesperadas são tratadas por handler global + rollback (quando aplicável).

---

### 🎨 Frontends (Angular 20)

Dois SPAs independentes, ambos com arquitetura moderna e foco em UX.

#### Web Admin (Gestão)

- Painel de produtividade (cadastros/gestão/controle)
- **Resolvers** para pré-carregar dados críticos antes de renderizar telas
- Guards por perfil de acesso (ex.: `adminOnly`, `employeeOnly`)

#### Web Portal (Cliente Final)

- Fluxo de autoatendimento (self-service)
- Integração com login Google (quando configurado)
- Interceptors para sessão resiliente: Bearer token + refresh automático em `401`

#### Stack moderna (comprovada no código)

- Standalone-style (sem NgModules): `bootstrapApplication(...)` e componentes com `imports: [...]`
- `provideZonelessChangeDetection()` habilitado
- Router com `withViewTransitions(...)`
- Transloco i18n completo: `pt-BR`, `en-US`, `es-ES`

---

## 🛠️ Stack Tecnológica

### Backend (Server)

- **Core:** .NET 8, ASP.NET Core Web API
- **Dados:** EF Core 8, SQL Server
- **Arquitetura:** Clean Architecture, CQRS (MediatR), DDD
- **Validação:** FluentValidation
- **Mapeamento:** AutoMapper
- **Auth:** ASP.NET Identity, JWT Bearer
- **Background Jobs:** Hangfire
- **Logging:** Serilog (envio de logs ao New Relic)
- **Storage:** AWS S3 SDK (integração com Cloudflare R2)

### Frontend (Web-Admin & Web-Portal)

- **Framework:** Angular 20
- **Linguagem:** TypeScript
- **Estilização:** SCSS
- **Componentes/Ícones:** FontAwesome & Bootstrap Icons
- **Async:** RxJS + AsyncPipe
- **i18n:** Transloco (@jsverse/transloco)

---

## 📂 Estrutura do Projeto

```text
/
├── server/                 # API .NET 8
│   ├── core/               # Domain + Application (regras + casos de uso)
│   ├── infrastructure/     # EF Core, repos, integrações externas
│   └── web-api/            # Controllers, DI, middlewares, Identity, jobs
│
├── web-admin/              # SPA Angular - Painel Administrativo
│   ├── src/app/routes/     # Rotas + lazy loading + guards
│   └── src/app/resolvers/  # Pré-carregamento de dados
│
└── web-portal/             # SPA Angular - Área do Cliente
    └── src/app/components/ # Componentes focados em UX
```

---

<a id="config"></a>

## ⚙️ Configuração (variáveis de ambiente)

A aplicação usa `IConfiguration` de duas formas:

1. **Chaves avulsas** (lidas direto):

- Ex.: `builder.Configuration["SQL_CONNECTION_STRING"]`

2. **Seções tipadas (Options Pattern)** via `services.Configure<T>(configuration.GetSection("..."))`:

- `APPURLS` → `IOptions<AppUrlsOptions>`
- `MAILOPTIONS` → `IOptions<MailSettings>`
- `CLOUDFLARE_R2_CREDENTIALS` → `IOptions<CloudflareR2Options>`

> Em variáveis de ambiente (Docker/CI), use `__` no lugar de `:` em chaves aninhadas
> (ex.: `MAILOPTIONS__HOST`, `APPURLS__ADMINAPP`).  
> Chaves são case-insensitive no provider de env vars do .NET.

<details>
<summary><strong>Ver variáveis obrigatórias</strong></summary>

### App URLs

| Chave (IConfiguration) | Env var (Docker/CI)  | Descrição                       |
| ---------------------- | -------------------- | ------------------------------- |
| `APPURLS:ADMINAPP`     | `APPURLS__ADMINAPP`  | URL do Admin (templates/links)  |
| `APPURLS:PORTALAPP`    | `APPURLS__PORTALAPP` | URL do Portal (templates/links) |

### Captcha

| Chave (IConfiguration) | Env var (Docker/CI) | Descrição                                     |
| ---------------------- | ------------------- | --------------------------------------------- |
| `CAPTCHA_KEY`          | `CAPTCHA_KEY`       | Chave do reCAPTCHA                            |
| `CAPTCHA_ADMIN`        | `CAPTCHA_ADMIN`     | Bypass para `PlatformAdmin` (use com cuidado) |

### Cloudflare R2

| Chave (IConfiguration)                      | Env var (Docker/CI)                          | Descrição  |
| ------------------------------------------- | -------------------------------------------- | ---------- |
| `CLOUDFLARE_R2_CREDENTIALS:ACCOUNTID`       | `CLOUDFLARE_R2_CREDENTIALS__ACCOUNTID`       | Account Id |
| `CLOUDFLARE_R2_CREDENTIALS:SERVICEURL`      | `CLOUDFLARE_R2_CREDENTIALS__SERVICEURL`      | Endpoint   |
| `CLOUDFLARE_R2_CREDENTIALS:ACCESSKEYID`     | `CLOUDFLARE_R2_CREDENTIALS__ACCESSKEYID`     | Access key |
| `CLOUDFLARE_R2_CREDENTIALS:SECRETACCESSKEY` | `CLOUDFLARE_R2_CREDENTIALS__SECRETACCESSKEY` | Secret key |
| `CLOUDFLARE_R2_CREDENTIALS:BUCKETNAME`      | `CLOUDFLARE_R2_CREDENTIALS__BUCKETNAME`      | Bucket     |

### CORS

| Chave (IConfiguration) | Env var (Docker/CI)    | Descrição                              |
| ---------------------- | ---------------------- | -------------------------------------- |
| `CORS_ALLOWED_ORIGINS` | `CORS_ALLOWED_ORIGINS` | Origens permitidas (separadas por `;`) |

### Banco / Jobs / Cache

| Chave (IConfiguration)           | Env var (Docker/CI)              | Descrição  |
| -------------------------------- | -------------------------------- | ---------- |
| `SQL_CONNECTION_STRING`          | `SQL_CONNECTION_STRING`          | SQL Server |
| `HANGFIRE_SQL_CONNECTION_STRING` | `HANGFIRE_SQL_CONNECTION_STRING` | Hangfire   |
| `REDIS_CONNECTION_STRING`        | `REDIS_CONNECTION_STRING`        | Redis      |

### JWT / Auth

| Chave (IConfiguration)           | Env var (Docker/CI)              | Descrição                                   |
| -------------------------------- | -------------------------------- | ------------------------------------------- |
| `JWT_GENERATION_KEY`             | `JWT_GENERATION_KEY`             | Assinatura JWT                              |
| `JWT_AUDIENCE_DOMAIN`            | `JWT_AUDIENCE_DOMAIN`            | Audience/issuer (valores separados por `;`) |
| `GOOGLE_CLIENT_ID`               | `GOOGLE_CLIENT_ID`               | Login Google                                |
| `LUCKYPENNYSOFTWARE_LICENSE_KEY` | `LUCKYPENNYSOFTWARE_LICENSE_KEY` | Licença                                     |

### E-mail

| Chave (IConfiguration)    | Env var (Docker/CI)        | Descrição       |
| ------------------------- | -------------------------- | --------------- |
| `MAILOPTIONS:HOST`        | `MAILOPTIONS__HOST`        | SMTP host       |
| `MAILOPTIONS:PORT`        | `MAILOPTIONS__PORT`        | SMTP port       |
| `MAILOPTIONS:USERNAME`    | `MAILOPTIONS__USERNAME`    | SMTP user       |
| `MAILOPTIONS:PASSWORD`    | `MAILOPTIONS__PASSWORD`    | SMTP pass       |
| `MAILOPTIONS:SENDERNAME`  | `MAILOPTIONS__SENDERNAME`  | Nome remetente  |
| `MAILOPTIONS:SENDEREMAIL` | `MAILOPTIONS__SENDEREMAIL` | Email remetente |

### Logging / Observability

| Chave (IConfiguration) | Env var (Docker/CI)    | Descrição     |
| ---------------------- | ---------------------- | ------------- |
| `NEWRELIC_LICENSE_KEY` | `NEWRELIC_LICENSE_KEY` | Envio de logs |

### Seed / Bootstrap

| Chave (IConfiguration)    | Env var (Docker/CI)       | Descrição                          |
| ------------------------- | ------------------------- | ---------------------------------- |
| `PLATFORM_ADMIN_FULLNAME` | `PLATFORM_ADMIN_FULLNAME` | Nome admin inicial                 |
| `PLATFORM_ADMIN_USERNAME` | `PLATFORM_ADMIN_USERNAME` | Username admin inicial             |
| `PLATFORM_ADMIN_EMAIL`    | `PLATFORM_ADMIN_EMAIL`    | Email admin inicial                |
| `PLATFORM_ADMIN_PASSWORD` | `PLATFORM_ADMIN_PASSWORD` | Senha do admin inicial (bootstrap) |

</details>

---

### Front-ends (Admin/Portal)

- `src/environments/environment.ts` (dev)
- `scripts/write-environment-prod.js` (build prod) lê:
  - `APIURL`
  - `CLIENT_ID`
  - `CAPTCHA_KEY`

> Essas variáveis precisam estar definidas para build/prod (script) e coerentes com a API.

---

## 🧪 Testes

- Suíte de testes em `server/tests/` com MSTest + Moq.

---

## 🧭 Documentação por projeto

- `server/` → detalhes de arquitetura, env vars e execução
- `web-admin/` → rotas, guards, resolvers, telas
- `web-portal/` → rotas, auth, i18n, telas

---

## 🚧 Melhorias Técnicas (Backlog)

- [ ] Segregação estrita de contratos (Response/DTOs bem separados por contexto)
- [ ] Response slimming (reduzir payloads e campos redundantes)
- [ ] Harmonização de nomenclatura (Models do Admin/Portal alinhados aos contratos do backend)
- [ ] Cache estratégico (Redis) para catálogos/configurações por tenant
- [ ] Testes E2E com Playwright

---

**Gustavo Santos**  
_Full Stack Developer .NET & Angular_
