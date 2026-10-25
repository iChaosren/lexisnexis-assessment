# Scaffold verification

Performed by Codex on Clayton-PC on **2026-10-25**, at the final repository path **C:\Github\lexisnexis-assessment**.

## Exact toolchain

| Component | Verified version |
| --- | --- |
| .NET SDK / target framework | 10.0.401 / net10.0 |
| ASP.NET Core runtime | 10.0.12 |
| Node / installed npm | 22.20.0 / 10.9.3 |
| Angular framework / compiler | 21.2.25 |
| Angular CLI / build | 21.2.28 |
| TypeScript / RxJS / tslib | 5.9.3 / 7.8.2 / 2.8.1 |
| Undici transitive override | 7.30.1 |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.12 |
| Microsoft.NET.Test.Sdk | 17.14.1 |
| xUnit / Visual Studio runner | 2.9.3 / 3.1.4 |
| Git for Windows used | 2.45.1.windows.1 |

Official Angular compatibility was checked against <https://angular.dev/reference/versions>. Angular 21 supports this Node and TypeScript combination. .NET 10 is an LTS release and meets the brief's .NET 8-or-later requirement.

## Completed checks

Commands below were run from the repository root (or the equivalent npm command from `frontend`).

| Check | Result |
| --- | --- |
| `dotnet restore .\LexisNexis.Assessment.slnx --locked-mode` | Passed for both projects. |
| `dotnet build .\LexisNexis.Assessment.slnx --no-restore --configuration Release` | Passed; 0 warnings, 0 errors. |
| `dotnet test .\LexisNexis.Assessment.slnx --no-build --configuration Release --list-tests` | Discovered exactly one test: `Assessment.Api.Tests.HealthEndpointTests.Health_endpoint_returns_healthy_from_the_real_api_pipeline`. |
| `dotnet test .\LexisNexis.Assessment.slnx --no-build --configuration Release` | Passed: 1; failed: 0; skipped: 0. |
| `npm.cmd --prefix .\frontend ci` using installed npm 10.9.3 | Passed against the final lockfile; 403 packages installed, 404 audited. |
| `npm.cmd --prefix .\frontend run build` | Passed; production output in `frontend/dist/assessment-web`; initial bundle 123.74 kB. |
| `npm.cmd --prefix .\frontend audit` | 0 reported vulnerabilities at verification time. |
| `npm.cmd --prefix .\frontend ls --depth=0` | All direct package versions matched the table above. |
| `npm.cmd --prefix .\frontend ls undici` | Both build/CLI dependency paths resolved to overridden 7.30.1. |

## Live development startup

The actual README commands `dotnet run --project .\backend\Assessment.Api` and `npm.cmd --prefix .\frontend start` were started in hidden temporary processes. Ports were checked before launch so existing listeners were not replaced.

Final successful smoke check: **2026-10-25T16:55:30Z**.

| Request | Result |
| --- | --- |
| `http://localhost:5080/api/health` | HTTP 200, body `Healthy`. |
| `http://localhost:4200/` | HTTP 200, Angular `<app-root>` present. |
| `http://localhost:4200/api/health` | HTTP 200, body `Healthy`, confirming the frontend development proxy reaches the API. |

All temporary server/listener processes started for verification were stopped. The final cleanup check confirmed **no listeners on ports 4200 or 5080**. No server is retained for preview.

This was an HTTP startup/proxy smoke check; it did not automate a browser click or assert rendered browser pixels.

## Setup issues resolved

- Sandboxed network resolution was unavailable, and a sandboxed .NET build exited without a useful diagnostic. Package restore and final verification succeeded outside the sandbox with scoped approval. Task-local npm/NuGet/.NET CLI caches were used under `C:\Users\myccb\Documents\Codex\2026-10-25\task\.tool-cache`; the repository source does not depend on those paths.
- Installed npm 10.9.3 initially crashed while resolving the generated dependency tree. A temporary `npx.cmd --yes npm@11.6.2 install` invocation generated a working lockfile without changing global npm. After adding the Undici override, `npm.cmd install --package-lock-only --ignore-scripts --no-audit --no-fund` reconciled optional native-package entries for npm 10; the final clean install with installed npm 10.9.3 passed.
- npm audit initially identified Angular build's pinned Undici 7.29.1. A same-major override to patched 7.30.1 resolved the reported advisories. See the [upstream advisory](https://github.com/nodejs/undici/security/advisories/GHSA-xjj6-f33q-929c). The final clean install, build, audit and startup/proxy check were repeated after this change.
- The Windows npm launcher left an Angular child listener after its parent exited during an early cleanup attempt. Its command line was verified against this scaffold before it was stopped. The final run explicitly tracked and stopped actual listener PIDs and passed cleanup.

## Coverage and delivery limits

- The single test covers infrastructure routing and health behavior. No business unit coverage, frontend test suite, order model, CRUD, validation, duplicate handling, totals, storage or status behavior exists yet.
- SOLUTION.md remains an unfilled template; no candidate decisions or video were fabricated.
- A fresh local Git repository was initialized on branch `main`; the existing normal Clayton identity was used for the scaffold commit. Generated dependency/build output is ignored.
- No remote repository, publication, sharing, submission, or recruiter contact was performed. A GitHub destination/account and visibility choice remain for a future remote-creation request.
