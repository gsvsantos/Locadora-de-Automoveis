# 🛠️ Web Admin — Angular 20 (Painel de Gestão)

SPA administrativa da plataforma **Locadora de Automóveis**. Aqui ficam as telas de operação interna (cadastros, gestão e controle), com **rotas protegidas por perfil** e uso intensivo de **Resolvers** para pré-carregar dados críticos.

> Para visão geral do sistema (Server + Admin + Portal), veja o README da raiz.

---

- Angular: `@angular/core` **^20.3.0**
- Rotas: **70**
- Telas documentadas: **53**
- Guards: **5** (ex.: `adminOnly`, `employeeOnly`, `platformAdminOnly`)
- Resolvers: **23**
- Services: **16**

---

## ⚡ Rodar localmente (dev)

### Pré-requisitos
- Node.js + npm (compatível com Angular CLI)
- Backend rodando (API)

### Subir o Admin
```bash
cd web-admin
npm install
npm start
````

> `npm start` executa `ng serve --port 4200`

---

## 🧩 Scripts

* `npm start` → `ng serve --port 4200`
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

## 🧭 Arquitetura do app (pontos relevantes)

* Angular moderno (standalone: `bootstrapApplication(...)` + `imports: [...]` nos componentes)
* Router com organização por áreas e lazy loading
* Uso forte de **Resolvers**:

  * melhora UX (tela já abre com dados),
  * mas aumenta o “custo” de navegação se você concentrar tudo em resolvers.
* Guards por perfil (admin/employee/platform admin)
* i18n com Transloco 

---

## 🔐 Autorização 

O Admin separa acessos por perfil; o padrão é:

* **Guard bloqueia cedo** (sem renderizar UI indevida)
* API valida de verdade (policy/roles) — o front é só a primeira linha

---

**Gustavo Santos**
*Full Stack Developer .NET & Angular*