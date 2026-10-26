# Implementation notes

This document describes the current code as inspected on 2026-10-26. It was drafted with AI assistance. It does not invent Clayton's personal rationale; candidate explanations and the final demonstration remain his to provide.

## Structure and storage

The backend targets .NET 10 and uses MVC controllers registered with `AddControllers` and `MapControllers`. Angular uses standalone components, a root-provided `ApiService`, reactive forms and Bulma styling. Backend DTOs are ordinary classes in `Dto`; the frontend has corresponding interfaces in `dtos`.

`IOrderDb` is registered as a singleton backed by `OrderDb` and its `ConcurrentDictionary<Guid, Order>`. All requests in one API process share that store. Two seeded orders contain the existing two customers. Customer listing derives distinct customers from those orders; it does not introduce another repository or new sample customers. Restarting the API loses changes and restores the seeds.

This keeps the current storage small and requires no database setup. It also means there is no durability or cross-process shared state. These are consequences of the implementation, not a claim about alternatives Clayton evaluated.

## Orders, identifiers and totals

`Guid.NewGuid(string)` hashes the exact external reference with MD5 to obtain a stable GUID. `TryAdd` returns `1` for an insertion and `0` when that key exists. The seeded order keys now use the same derivation. References are not trimmed, normalized or scoped by customer.

Creation resolves an existing customer and attaches its entity and ID. Line quantities/prices come from the request; totals do not. Entity properties compute quantity times unit price, sum the line totals, apply 15% tax, and add subtotal and tax. Backend arithmetic uses `decimal`. There is no explicit tax or rounding policy beyond those expressions; the detail view's two-decimal formatting changes presentation only.

Orders are listed by descending creation time. The list returns full order DTOs; the existing summary DTO is not used by that endpoint. A missing single order currently becomes HTTP 204 through MVC's null response formatting.

## Forms and customer selection

The create component owns its non-nullable FormGroup and one-line FormArray. The template uses reactive-form bindings and `ngSubmit`; the handler subscribes to the API request and displays insertion/duplicate/error feedback. The customer selector reads `/api/customers`. The existing `oneOf` validator is now in its own file with unchanged matching behavior.

List, detail-modal and status-selection implementation was led by Clayton. The assistant later connected creation and customer selection, including submission state/messages. Clayton said the submission work went slightly beyond what he intended and flagged `OrderDb` readability. This documentation/validation pass does not refactor either area.

## Validation and status behavior

The order controller now has `[ApiController]`. Its [standard filter](https://learn.microsoft.com/aspnet/core/web-api#automatic-http-400-responses) returns 400 for invalid ModelState before the action executes. Request quantity/price annotations are on properties, and JSON fractional quantities fail integer binding. Previously the missing attribute allowed invalid values into the action and a null DTO dereference produced 500 for fractional input. No custom number parser or manual ModelState workaround was added.

Status requests now validate `NewStatus` with `[EnumDataType(typeof(OrderStatus))]`; undefined values such as `999` return 400 `ValidationProblemDetails`. The action also returns 400 validation details when its route and body order IDs differ, without changing either order. For matching IDs, any differing defined status can replace the existing status and update its timestamp. Missing orders and unchanged statuses return `0`. Clayton commented out the previous Pending/terminal restrictions; they remain disabled. The supplied brief asks for sensible status behavior and feedback but does not prescribe those particular transitions.

## Verification and remaining work

See [VERIFICATION.md](VERIFICATION.md) for dated build/test/browser results. The latest recorded backend run on 2026-10-26 passed all 30 tests, including undefined-status and ID-mismatch rejection. Direct controller tests cover store/action behavior; HTTP tests cover MVC binding, validation and serialization. A passing infrastructure check is not represented as complete business coverage.

The direct detail URL lacks parameter-to-input wiring. There is no persisted store, multi-line UI editor or maintained frontend unit-test suite. These are recorded limits; no extra implementation is included in this documentation pass.

Potential follow-up decisions are reference normalization, tax/rounding assumptions and user-facing status feedback. These are recommendations for review, not implemented changes or fabricated candidate reasoning. Client filtering is optional in the supplied brief.

## AI assistance and final delivery

[AI_ASSISTANCE.md](AI_ASSISTANCE.md) records the substantial assistant work, overbuilt frontend and rollback, subsequent user-led implementation, later requested additions, and validation repair. It distinguishes exact prompt excerpts from summaries.

Clayton should review these factual notes and add his own explanation of decisions and tradeoffs. He confirms that the repository is public and the AI log has been reviewed. The final 5-10 minute narrated walkthrough is still outstanding, and its link has not been supplied in this repository. The assistant has not performed the final submission.
