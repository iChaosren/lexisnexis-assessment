# LexisNexis assessment scaffold

Setup only for Clayton's Order Intake & Tracking assessment. The application currently has an Angular starter page and one ASP.NET Core health endpoint. All assessment features and business tests are still to be implemented by Clayton.

## Prerequisites

Verified on Clayton-PC on 2026-10-25:

| Tool | Version |
| --- | --- |
| .NET SDK | 10.0.401 (LTS; pinned in `global.json`) |
| ASP.NET Core runtime | 10.0.12 |
| Node.js | 22.20.0 |
| npm | 10.9.3 |
| Angular framework | 21.2.25 |
| Angular CLI / build tools | 21.2.28 (project-local) |
| TypeScript | 5.9.3 |
| RxJS | 7.8.2 |

Angular 21 supports Node `^20.19.0 || ^22.12.0 || ^24.0.0` and TypeScript `>=5.9.0 <6.0.0`. These versions meet the assessment's Angular 18+, TypeScript 5+, and .NET 8+ requirements. No global Angular CLI install is needed. See [Angular compatibility](https://angular.dev/reference/versions) and [Microsoft's .NET 10 overview](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview).

The package override pins Angular's transitive build dependency `undici` to patched version 7.30.1; Angular build 21.2.28 otherwise pins 7.29.1. Revisit this override when upgrading Angular's build tools. See the [upstream security advisory](https://github.com/nodejs/undici/security/advisories/GHSA-xjj6-f33q-929c).

## Install

Run these commands in Windows PowerShell. `npm.cmd` avoids PowerShell execution-policy issues with `npm.ps1`.

```powershell
Set-Location C:\Github\lexisnexis-assessment
dotnet restore .\LexisNexis.Assessment.slnx --locked-mode
npm.cmd --prefix .\frontend ci
```

The .NET solution contains the API and its test project. The Angular project lives in `frontend` and has its own package lock. Package restore requires internet access on the first run.

## Run locally

Terminal 1, from the repository root:

```powershell
dotnet run --project .\backend\Assessment.Api
```

Terminal 2, from the repository root:

```powershell
npm.cmd --prefix .\frontend start
```

Open <http://localhost:4200> and click **Check API health**. With both servers running, the page should display `Healthy`. The API listens at <http://localhost:5080/api/health>. These are local HTTP development servers; no certificate trust setup is required.

Angular's development proxy forwards `/api/**` to port 5080. It is configured in `frontend/src/proxy.conf.json` and referenced by `frontend/angular.json`. A production deployment will need its own routing configuration. See [Angular proxy documentation](https://angular.dev/tools/cli/serve#proxying-to-a-backend-server).

Optional PowerShell smoke check while both terminals are running:

```powershell
Invoke-RestMethod http://localhost:5080/api/health
Invoke-RestMethod http://localhost:4200/api/health
```

Both should return `Healthy`. Stop each server with **Ctrl+C** in its terminal.

## Build and test

From the repository root, after install:

```powershell
dotnet build .\LexisNexis.Assessment.slnx --no-restore --configuration Release
dotnet test .\LexisNexis.Assessment.slnx --no-build --configuration Release --list-tests
dotnet test .\LexisNexis.Assessment.slnx --no-build --configuration Release
npm.cmd --prefix .\frontend run build
```

There is **one infrastructure integration smoke test** in `backend/Assessment.Api.Tests/HealthEndpointTests.cs`. It hosts the real API pipeline in memory and checks that `/api/health` returns HTTP 200 and `Healthy`. It is not business unit coverage. No empty tests or frontend test suite are included. Clayton still needs to add the assessment's backend unit tests as he implements the behavior.

## A short React-to-Angular file map

| React concept | This scaffold |
| --- | --- |
| `createRoot(...)` entry point | `frontend/src/main.ts` bootstraps the standalone `App` component. |
| `App.tsx` component logic | `frontend/src/app/app.ts`; `@Component` connects the class, template, and CSS. Standalone is Angular's default; there is no `AppModule`. |
| JSX markup | `frontend/src/app/app.html`; `{{ health() }}` displays a signal, and `(click)="checkHealth()"` binds an event. |
| Component state | `signal(...)` in `app.ts`; read with `health()` and update with `health.set(...)`. |
| Context/providers | `frontend/src/app/app.config.ts`; `provideHttpClient()` makes Angular's HTTP service injectable. |
| A request using `fetch` | `inject(HttpClient)` and `http.get(...)` in `app.ts`; this small one-response request uses an Observable subscription. |
| Component/global CSS | `frontend/src/app/app.css` / `frontend/src/styles.css`. |
| Vite config and dev proxy | `frontend/angular.json` / `frontend/src/proxy.conf.json`. |
| `package.json` scripts | `frontend/package.json`; `npm.cmd start` uses the local Angular CLI. |

The health example demonstrates only component state, event binding, dependency injection, and a local API call. Routing, forms, storage, orders, validation, duplicate handling, totals, and status flows have not been implemented.

## Assessment handoff

- `SOLUTION.md` is an unfilled decision template for Clayton to complete in his own words.
- `AI_ASSISTANCE.md` records the scaffold assistance and prompts honestly; continue the log if using more AI assistance.
- `VERIFICATION.md` records actual setup checks and their limits.
- Final delivery still needs the completed GitHub repository, README, solution decisions/tradeoffs, and a narrated 5–10 minute video.

This is a local Git repository. No remote destination has been chosen or created, and nothing has been published or submitted.
