# 🌐 Web Portal — Angular 20 (Self-Service do Cliente)

SPA do cliente final (autoatendimento) da plataforma **Locadora de Automóveis**. O foco aqui é UX e um fluxo de sessão resiliente (login, refresh, rotas protegidas), com o menor “peso” possível comparado ao Admin.

> Para visão geral do sistema (Server + Admin + Portal), veja o README da raiz.

---

- Angular: `@angular/core` **^20.3.0**
- Componentes: **13**
- Arquivos de rotas: **4**
- Guards: **2**
- Interceptors: **2**
- Resolvers: **9**
- Services HTTP: **10**

---

## ⚡ Rodar localmente (dev)

### Pré-requisitos
- Node.js + npm (compatível com Angular CLI)
- Backend rodando (API)

### Subir o Portal
```bash
cd web-portal
npm install
npm start
````

> `npm start` executa `ng serve --port 4201`

---

## 🧩 Scripts

* `npm start` → `ng serve --port 4201`
* `npm run build` → `ng build`
* `npm run watch` → `ng build --watch --configuration development`
* `npm run format` → `prettier --write .`
* `npm run build:prod` → `npm run prebuild:prod && ng build --configuration production`

---

## ⚙️ Configuração de ambiente

### Dev

Arquivo:

* `src/environments/environment.ts`

Chaves usadas:

* `production`
* `apiUrl`
* `client_id`
* `captcha_key`

### Build de produção

O build prod injeta as configs em build time via:

* `scripts/write-environment-prod.js`

Esse script lê as env vars:

* `APIURL`
* `CLIENT_ID`
* `CAPTCHA_KEY`

---

## 🧭 Rotas principais (alto nível)

* Auth:

  * login
  * register
  * forget-password
  * reset-password
* Home
* Account:

  * details
  * edit
* Rentals:

  * list
  * new
  * details

---

## 🔐 Sessão e autenticação

O portal mantém a experiência do usuário consistente com:

### Interceptor de autenticação

* adiciona Bearer token nas chamadas
* tenta refresh automático em `401` (quando o backend está configurado)

### Interceptor de idioma

* garante consistência de cultura/i18n nas requisições (ex.: header/idioma)

---

## 🌍 i18n

O projeto usa Transloco e segue o padrão de idiomas:

* `pt-BR`
* `en-US`
* `es-ES`

---

**Gustavo Santos**
*Full Stack Developer .NET & Angular*