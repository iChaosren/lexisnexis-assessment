# Order Intake & Tracking

ASP.NET Core controller API with an Angular standalone frontend. The application submits orders for either seeded customer, lists the newest orders first, shows details in a modal, and changes order status. It uses an in-memory store; restarting the API restores the two seeded orders and discards later changes.

See [SOLUTION.md](SOLUTION.md) for implementation facts and tradeoffs, [VERIFICATION.md](VERIFICATION.md) for dated check results and known limits, and [AI_ASSISTANCE.md](AI_ASSISTANCE.md) for AI involvement. This is a local development application, with known limitations listed below.

## Toolchain

| Tool | Version used |
| --- | --- |
| .NET SDK / ASP.NET Core runtime | 10.0.401 / 10.0.12 |
| Node.js / pnpm | 22.20.0 / 12.14.0 |
| Angular framework / CLI and build | 21.2.25 / 21.2.28 |
| TypeScript / RxJS / Bulma | 5.9.3 / 7.8.2 / 1.0.4 |

`global.json` selects the .NET 10 SDK with patch roll-forward. Frontend versions and pnpm are recorded in `frontend/package.json`; `pnpm-lock.yaml` is the active lockfile. No global Angular CLI installation is needed.

## Install and run on Windows

From the checkout root (the local path used during development is shown):

```powershell
Set-Location C:\Github\lexisnexis-assessment
dotnet restore .\backend\LexisNexis.Assessment.sln --locked-mode
pnpm.cmd --dir .\frontend install --frozen-lockfile
```

Start the API in one terminal:

```powershell
dotnet run --project .\backend\Assessment.Api --launch-profile http
```

Start Angular in another terminal, also from the checkout root:

```powershell
pnpm.cmd --dir .\frontend start
```

Open <http://localhost:4200>. The API listens on <http://localhost:5080>; <http://localhost:5080/api/health> returns plain-text `Healthy`. Angular forwards `/api/**` to port 5080 using `frontend/src/proxy.conf.json`. Stop each server with Ctrl+C. Restarting the API clears its in-memory changes; an existing process must load new code before newly added endpoints are available.

Use **New order**, choose a customer and currency, fill the line item, then submit. The form displays success, duplicate-reference or error feedback. **Orders** shows the list; **View** opens its detail modal, and the row's status select sends an update.

## Build and test

```powershell
dotnet build .\backend\LexisNexis.Assessment.sln --no-restore --configuration Release
dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release
pnpm.cmd --dir .\frontend build
```

The test suite contains direct controller tests and focused HTTP integration checks. Automatic MVC binding/validation is checked over HTTP; direct action calls do not execute MVC filters. The latest recorded backend run on 2026-10-26 passed all 30 cases with no failures or skips: see [VERIFICATION.md](VERIFICATION.md). No maintained frontend unit-test target is configured; the recorded browser smoke check is separate.

## API and contracts

| Method and path | Current response |
| --- | --- |
| `GET /api/health` | 200, plain-text `Healthy` |
| `GET /api/customers` | 200, `CustomerResponse[]` for the two existing customers |
| `POST /api/orders` | 200, `1` when inserted or `0` for an existing external reference |
| `GET /api/orders` | 200, full `OrderResponse[]`, newest first |
| `GET /api/orders/{id:guid}` | 200, `OrderResponse`; a missing order currently returns 204 |
| `PATCH /api/orders/{id:guid}/status` | 200, `1` when changed or `0` for a missing order/unchanged status; 400 for an undefined status or mismatched IDs |

Create requests contain `externalReference`, `customerId`, `currency`, and `lines`. Each line has `sku`, `name`, integer `quantity`, and decimal `unitPrice`. For example:

```json
{
  "externalReference": "EXAMPLE-001",
  "customerId": "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa",
  "currency": "ZAR",
  "lines": [
    { "sku": "ITEM-1", "name": "Example item", "quantity": 2, "unitPrice": 10 }
  ]
}
```

The other seeded customer ID is `bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb`; the UI obtains both from `/api/customers`. An unknown or empty customer ID returns 400 with `Choose an existing customer.`

Responses include the customer, timestamps, status, currency, lines, line totals, subtotal, tax and total. IDs are deterministically derived from the exact external-reference string by the current `Guid.NewGuid(string)` helper. Obtain an order's ID from the list response; do not use the obsolete literal GUIDs from the early scaffold.

JSON property names use camel case. Enums write their declared names: `Pending`, `Confirmed`, `Fulfilled`, `Cancelled`; currencies are `ZAR`, `EUR`, `USD`. The converter also accepts numeric values, but `NewStatus` additionally uses `[EnumDataType(typeof(OrderStatus))]` to reject undefined statuses such as `999`. Status requests contain `orderId` and `newStatus`; the body ID must match the route ID. An undefined status or ID mismatch returns 400 `ValidationProblemDetails` without changing an order.

`OrderController` uses `[ApiController]`: failed JSON binding or request validation returns 400 `ValidationProblemDetails`. Existing line annotations require quantity >= 1 and price >= 0; fractional JSON quantities cannot bind to an integer. The frontend's validators are UX checks, not a substitute for this server handling.

## Code map

| Area | Files |
| --- | --- |
| Backend solution | `backend/LexisNexis.Assessment.sln` |
| MVC registration, JSON options, singleton store | `backend/Assessment.Api/Program.cs` |
| Order and customer routes | `backend/Assessment.Api/Controllers/` |
| In-memory implementation / abstraction | `backend/Assessment.Api/Db/OrderDb.cs`, `Interfaces/IOrderDb.cs` |
| Entities, mappings and totals | `backend/Assessment.Api/Entities/` |
| Request/response classes and enums | `backend/Assessment.Api/Dto/`, `Enums/` |
| Controller and HTTP tests | `backend/Assessment.Api.Tests/` |
| Angular bootstrap / app shell | `frontend/src/main.ts`, `frontend/src/app/app.ts` and `.html` |
| Routes / DI providers | `frontend/src/app/app.routes.ts`, `app.config.ts` |
| Root HTTP service / interfaces | `frontend/src/app/api.service.ts`, `dtos/` |
| Component classes and templates | `frontend/src/app/orders/` |
| Extracted validator / global styling | `frontend/src/app/one-of.validator.ts`, `frontend/src/styles.css` |

For component, DI and form details, see [frontend/LEARNING.md](frontend/LEARNING.md).

## Current limits

- Storage is process-local, with no persistence or authentication. Customers are derived from the existing orders; there is no customer editor.
- Reference matching uses the exact string without trimming or case normalization. The form edits one line; it has no add/remove-line controls.
- The server applies 15% tax for every currency and does not apply an explicit monetary rounding policy. The frontend formats detail totals to two decimal places.
- No status transition restrictions are active between defined statuses. The earlier Pending/terminal checks are commented out; those specific rules are not treated as assessment requirements.
- Status failures are logged to the browser console rather than shown as detailed UI feedback.
- The detail modal works through the list's selected ID. The declared `/orders/:id` route does not currently bind its route parameter to the component input.
- Release builds retain nullable warnings and a frontend bundle-budget warning. Verification is recorded with its date and limitations, not represented as complete business coverage.

Clayton confirms that the repository is public and the AI log has been reviewed. The narrated walkthrough is still outstanding.
