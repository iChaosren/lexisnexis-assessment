# LexisNexis assessment learning scaffold

Open **`backend\LexisNexis.Assessment.sln`** for the backend. It contains the API and its test project. The first scaffold used a root `.slnx` because .NET 10's template defaults to that newer solution format. It has now been replaced by one conventional `.sln` inside `backend`; the old file remains recoverable in Git commit `b58bb19`. There are no competing solution files to maintain.

This remains boilerplate for Clayton's Order Intake & Tracking assessment. Four backend endpoint stubs and three Angular pages are connected, with comments quoting the supplied assessment excerpts next to the relevant extension points. **Every order endpoint returns HTTP 501 Not Implemented.** No order is saved and no business rules have been implemented.

## Installed toolchain

| Tool | Version used |
| --- | --- |
| .NET SDK / ASP.NET Core runtime | 10.0.401 / 10.0.12 |
| Node.js / pnpm | 22.20.0 / 12.14.0 |
| Angular framework, forms, router | 21.2.25 |
| Angular CLI / build | 21.2.28 (project-local) |
| TypeScript / RxJS | 5.9.3 / 7.8.2 |

Angular 21 supports the installed Node and TypeScript versions. No global CLI, SDK, or package-manager change is needed. See [Angular compatibility](https://angular.dev/reference/versions) and [.NET 10](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview).

## Install

Run in Windows PowerShell. The `.cmd` commands avoid PowerShell execution-policy issues with script wrappers.

```powershell
Set-Location C:\Github\lexisnexis-assessment
dotnet restore .\backend\LexisNexis.Assessment.sln --locked-mode
pnpm.cmd --dir .\frontend install --frozen-lockfile
```

`frontend/pnpm-lock.yaml` is the active frontend lockfile. It was imported from the original npm lock; that original is recoverable in `b58bb19`, rather than kept as a second active lock. Package metadata and Angular's CLI configuration now select pnpm. `frontend/pnpm-workspace.yaml` holds pnpm 12's local settings for this single frontend package: the existing patched Undici override, and named native build/cache/watch dependencies permitted to run install scripts. This does not introduce a multi-project architecture or change global settings.

## Run

Terminal 1, from the repository root:

```powershell
dotnet run --project .\backend\Assessment.Api
```

Terminal 2, from the repository root:

```powershell
pnpm.cmd --dir .\frontend start
```

Open <http://localhost:4200>. The shell's **Check API health** button should display `Healthy`. The API health URL is <http://localhost:5080/api/health>. Both servers use local HTTP; no certificate trust step is needed. Stop each terminal with **Ctrl+C**.

| Angular route | Learning stub |
| --- | --- |
| `/orders` | Calls the list endpoint and displays the expected 501 TODO feedback; has a reload button and an eventual response template. |
| `/orders/new` | Local FormBuilder form, one input line, add-line example, per-field errors, and submit request wiring. |
| `/orders/:id` | Loads the route ID, calls detail, and includes optional free-text change-status request wiring. |

The list page links to an explicitly labelled placeholder GUID so the detail stub can be explored before any orders exist. It is not a saved order. There are no sample success responses, invented statuses, or calculated totals.

Angular's dev proxy forwards `/api/**` to `localhost:5080`, using `frontend/src/proxy.conf.json`. A production deployment will need its own routing. See [Angular proxy configuration](https://angular.dev/tools/cli/serve#proxying-to-a-backend-server).

## Backend extension points and provisional contracts

Start in `backend/Assessment.Api/OrdersEndpoints.cs`; `Program.cs` calls `MapOrdersEndpoints()`. Each handler contains its own explicit TODO and 501 ProblemDetails response. Change those methods as you implement, rather than adding layers just to fill in the scaffold.

| Method and path | Handler | Request / intended response shape |
| --- | --- | --- |
| `POST /api/orders` | `CreateOrder` | `CreateOrderRequest` / `OrderResponse` |
| `GET /api/orders` | `ListOrders` | no body / `OrderSummaryResponse[]` |
| `GET /api/orders/{id:guid}` | `GetOrder` | GUID route ID / `OrderResponse` |
| `PATCH /api/orders/{id:guid}/status` | `ChangeOrderStatus` | `ChangeOrderStatusRequest` / `OrderResponse` |

`OrderContracts.cs` and `frontend/src/app/order-contracts.ts` contain corresponding shapes. **The route conventions, field names, GUID identifiers, UTC timestamp field, required UI reference/description examples, and plain-string status are scaffold choices made with AI assistance, not mandated assessment details.** Revise them as you design. Requests carry `clientReference` and lines (`description`, `quantity`, `unitPrice`), or a `status` string. The future response shapes include server totals; no calculation or successful response is currently supplied. Decimal handling/rounding and final error behavior remain your decisions.

ASP.NET's ordinary route/JSON binding can reject malformed IDs or JSON before a stub runs. No custom backend quantity/price validation, duplicate handling, sorting, persistence, totals, status vocabulary, or transition policy has been added.

## Angular learning map

| React / application concept | Actual file |
| --- | --- |
| Application entry point | `frontend/src/main.ts` bootstraps the standalone `App` class. |
| App shell / JSX equivalent | `frontend/src/app/app.ts` with `app.html` and `app.css`; its template contains navigation and RouterOutlet. |
| Route definitions | `frontend/src/app/app.routes.ts`; literal `new` precedes the `:id` route. |
| App providers / dependency configuration | `frontend/src/app/app.config.ts`. |
| Shared HTTP client wrapper | `frontend/src/app/api.service.ts`, root-provided and injecting HttpClient. |
| Request/response types | `frontend/src/app/order-contracts.ts`; backend counterpart `OrderContracts.cs`. |
| List page | `frontend/src/app/orders/order-list.ts` and `.html`. |
| Form page and field errors | `frontend/src/app/orders/order-create.ts` and `.html`. |
| Detail and optional status form | `frontend/src/app/orders/order-detail.ts` and `.html`. |
| Shared API error display wording | `frontend/src/app/api-error.ts`. |
| Global styles / build configuration | `frontend/src/styles.css` / `frontend/angular.json`. |

Read **[frontend/LEARNING.md](frontend/LEARNING.md)** for the concrete DI, FormBuilder, template-import and HTTP walkthrough. FormBuilder can be injected into each form component; each component creates and owns its own FormGroup. A root ApiService shares request methods, not form state. `inject(ApiService)` obtains a DI-managed instance; importing its TypeScript class or adding template imports does something different. Constructor injection and inline templates are valid alternatives; field injection and separate templates are used consistently here.

The frontend Validators are requested UX examples. Per-field errors live on each control and are shown by that field's template after touched/dirty or submission. They do not replace the backend validation you still need to implement.

## Build and test

From the repository root after install:

```powershell
dotnet build .\backend\LexisNexis.Assessment.sln --no-restore --configuration Release
dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release --list-tests
dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release
pnpm.cmd --dir .\frontend run build
pnpm.cmd --dir .\frontend audit
```

The backend suite has five infrastructure cases: one health test and four endpoint wiring cases asserting operation-specific 501 ProblemDetails. **They are not business unit coverage.** The TODO comment points to the brief's requirement to test the critical business rules you eventually implement. There are no empty or pretend-passing business tests and no frontend unit-test suite. Browser smoke checks performed during setup are recorded separately in `VERIFICATION.md`.

Quick health/proxy check while running both servers:

```powershell
Invoke-RestMethod http://localhost:5080/api/health
Invoke-RestMethod http://localhost:4200/api/health
```

Both return `Healthy`. Calls to the order stubs should currently receive 501, which is intentional.

## Remaining assessment work

Implement the business behavior, backend validation and business tests yourself. Decide storage, duplicate/reference semantics, totals/rounding, statuses/transitions, final HTTP errors, and completed UI behavior. `SOLUTION.md` remains an unfilled template for your actual decisions. Continue `AI_ASSISTANCE.md` with later prompts and your review. Final delivery still needs the completed repository and narrated 5–10 minute video.

This task made local changes only. No remote was created and nothing was pushed, published, shared or submitted by the assistant. Any existing Git remote configuration is preserved.
