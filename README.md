# SaaS Boilerplate

A production-ready **multi-tenant SaaS starter** monorepo: **.NET 10** minimal APIs on the backend, **Vue 3** + **Vuetify 3** on the frontend, and **PostgreSQL** with **row-level security (RLS)** for tenant isolation.

## What you get

| Layer | Stack | Highlights |
| ------- | -------- | ------------ |
| **API** | ASP.NET Core 10, EF Core 10 | Vertical slices, JWT auth, workspace-slug login, Serilog, global exception handling, QuestPDF infrastructure |
| **Data** | PostgreSQL 16 (Docker) | RLS + EF query filters, DB-backed roles, Stripe-style display IDs (`ten_…`, `usr_…`) |
| **Web** | Vue 3, Vite, Pinia, vue-i18n | Workspace login (no GUID), JWT session, API interceptors, dashboard shell (English UI) |
| **Tests** | xUnit, FluentAssertions | Sample tests for display ID generation; `Microsoft.AspNetCore.Mvc.Testing` ready for integration tests |

## Repository layout

```text
SaaS.Boilerplate/
├── docker-compose.yml                    # PostgreSQL 16 for local development
├── SaaS.Boilerplate.sln
├── .vscode/tasks.json                    # IDE tasks for setup, migrate, and run
├── src/
│   ├── Api/
│   │   ├── appsettings.Development.example.json   # Copy to appsettings.Development.json (gitignored)
│   │   └── …                            # Backend (SaaS.Api)
│   ├── Tests/                           # xUnit (SaaS.Api.Tests)
│   └── Web/                             # Frontend (Vue 3)
├── Directory.Packages.props
├── Directory.Build.props
└── global.json
```

## Quick start (first run)

From the repository root, in order:

1. **PostgreSQL** — `docker compose up -d` (see [Database (Docker)](#database-docker) for template credentials).
2. **Local secrets** — copy `src/Api/appsettings.Development.example.json` → `src/Api/appsettings.Development.json` and set your Postgres password (same as Docker) and a unique `Jwt:Secret` (32+ characters). This file is **gitignored**.
3. **Dependencies** — IDE task **1. Setup: Restore & Install**, or `dotnet restore` + `yarn install` manually.
4. **Schema** — IDE task **2. Database: Update**, or `dotnet ef database update --project src/Api/SaaS.Api.csproj`.
5. **Run** — tasks **3** and **4** (API and Web in separate terminals). On first API start, the seeder creates roles, workspace `system`, and `admin@system.com` / `Admin123!`.
6. **Sign in** at <http://localhost:3000> — workspace `system`, email `admin@system.com`, password `Admin123!`.

Optional: `src/Web/.env.example` → `src/Web/.env` if you need a custom API proxy target.

## Developer workflow (IDE tasks)

Open this folder as your **workspace root** (`SaaS.Boilerplate`) so `.vscode/tasks.json` is picked up.

**Command Palette** → **Tasks: Run Task** (or **Terminal** → **Run Task…**). Complete [Quick start](#quick-start-first-run) steps 1–2 before **2. Database: Update**.

| Task | What it does |
| ------ | ---------------- |
| **1. Setup: Restore & Install** | `dotnet restore` in `src/Api` and `yarn install` / `npm install` in `src/Web` |
| **2. Database: Update** | `dotnet ef database update` in `src/Api` (requires running Postgres + `appsettings.Development.json`) |
| **3. Run: Backend API** | `dotnet run` in `src/Api` (background terminal) |
| **4. Run: Frontend** | `yarn dev` or `npm run dev` in `src/Web` (background terminal) |

Run **3** and **4** in separate task invocations so API and UI stay up together.

### Database (Docker)

```bash
docker compose up -d
```

| Setting | Value (template / committed defaults) |
| ------- | ------------------------------------- |
| Host port | `5432` |
| Database | `saas_db` |
| User | `postgres` |
| Password | `ChangeMeQuickly1990@` |

Data is stored in the `saas_postgres_data` volume. Replace the password in `docker-compose.yml` and in your **local** `appsettings.Development.json` before shared use (see [Disclaimer](#disclaimer)).

### Configuration notes

Committed `appsettings.json` and `docker-compose.yml` use **placeholder** values only. Real secrets belong in gitignored `appsettings.Development.json`.

**PowerShell (Windows):**

```powershell
Copy-Item src/Api/appsettings.Development.example.json src/Api/appsettings.Development.json
```

**bash:**

```bash
cp src/Api/appsettings.Development.example.json src/Api/appsettings.Development.json
```

The UI shows human-readable display IDs (`ten_…`, `usr_…`); the JWT still carries internal GUIDs for API tenant scoping.

### Default dev URLs

| Service | URL |
| --------- | ----- |
| API | <http://localhost:5080> |
| Web (Vite) | <http://localhost:3000> |
| API docs (dev) | <http://localhost:5080/scalar/v1> |

The Vite dev server proxies `/api` to the backend (see `src/Web/.env.example`).

### Manual commands (optional)

From the repository root:

```bash
dotnet restore src/Api/SaaS.Api.csproj
cd src/Web && yarn install
dotnet ef database update --project src/Api/SaaS.Api.csproj
dotnet run --project src/Api/SaaS.Api.csproj
cd src/Web && yarn dev
dotnet test SaaS.Boilerplate.sln
```

### Logging

The API uses **Serilog** with structured console output. Levels are configured in `appsettings.json` (and overridden in Development). Fatal startup errors are logged before exit.

## Architecture principles

- **Vertical slices** under `src/Api/Features/` — no MediatR, no generic repository.
- **EF Core** `AppDbContext` injected directly into feature handlers.
- **Immutability** — DTOs as C# `record` types; Vue API types aligned with backend responses.
- **Tenant context** — `TenantMiddleware` + PostgreSQL RLS session variables + EF query filters.

## License

Use and adapt freely for your own SaaS products. Replace secrets, branding, and seed data before production.

## Disclaimer

This repository is a **public starter template** provided **“AS IS”**, without warranty of any kind, express or implied, including but not limited to merchantability, fitness for a particular purpose, or non-infringement. The authors and contributors are **not liable** for any damages, data loss, security incidents, or downtime arising from the use or misuse of this code.

By cloning, forking, or deploying this project, **you accept full responsibility** for how it is configured, operated, and secured in your environment.

### Default credentials and secrets (change before any shared or production use)

This boilerplate ships with **known, documented defaults** intended for **local development only**. They are **not safe** for staging, demos on the public internet, or production.

**You must replace all of the following before exposing the system to anyone other than yourself on a trusted machine:**

| Item | Default (examples in this repo) | Action required |
| ------ | --------------------------------- | ----------------- |
| PostgreSQL password | `ChangeMeQuickly1990@` in `docker-compose.yml`, `appsettings.json`, and `appsettings.Development.example.json` | Replace everywhere with a strong unique password; keep real values only in **gitignored** `appsettings.Development.json` |
| PostgreSQL user / database | `postgres` / `saas_db` | Use least-privilege users and separate databases per environment where appropriate |
| JWT signing secret | Placeholder in committed config files | Generate a long random secret (32+ characters) in your local `appsettings.Development.json` only |
| Seeded admin account | `admin@system.com` / `Admin123!` | Delete, disable, or change password immediately; prefer creating your own admin |
| Default workspace slug | `system` | Expected for local login; treat as public knowledge in this template |

Failure to rotate these values may allow **unauthorized database access**, **token forgery**, or **account takeover**.

### Security and compliance

- Do **not** deploy this project unchanged to a network-accessible or production environment.
- Review authentication, authorization, RLS policies, logging, and data handling for your jurisdiction and industry (e.g. GDPR, HIPAA) — **this template does not certify compliance**.
- Scan dependencies and images regularly; pin and update versions at your own cadence.
- Never commit real passwords, API keys, or production connection strings to Git. Use user secrets, environment variables, or a vault.

### No official support

This is an **open-source boilerplate**, not a supported product. Issues and contributions are welcome on a best-effort basis; there is **no SLA** or guarantee of fixes, updates, or security advisories.

If you do not agree with these terms, **do not use** this repository.
