# Verification record

Updated 2026-10-26. Results below describe the stated source snapshot and checks, not a claim of complete business coverage. The initial health/501 scaffold records are historical and remain in Git history; they no longer describe this application.

## Current backend checks

On the current validation-fixed source (2026-10-26), the Release build passed with 0 errors and 5 existing CS8618 nullable warnings in `Order`/`LineItem`. The full suite ran 27 cases: **26 passed, 1 failed, 0 skipped**.

Quantities `0`/`-1`, price `-0.01`, and fractional quantity `1.5` now return 400 with typed `ValidationProblemDetails`. Tests that assumed the removed Pending/terminal restrictions now check the actual unrestricted status behavior and pass.

The remaining failing test is `OrderEndpointTests.RejectsUnknownStatus`: numeric status `999` returns 200. Neither a status-value validation annotation nor stricter numeric-enum parsing was added; this is separate from normal ModelState handling and transition policy.

Commands run from `C:\Github\lexisnexis-assessment`:

```powershell
dotnet build .\backend\LexisNexis.Assessment.sln --no-restore --configuration Release
dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release
```

Direct controller tests cover creation, repeated references, newest-first listing, detail/totals, customer association and status updates. They do not execute MVC model-binding/filter behavior. HTTP tests cover serialization, missing responses, customer listing/rejection and invalid request handling through `WebApplicationFactory` with fresh in-memory stores.

Validation repair: `[ApiController]` now returns standard 400 `ValidationProblemDetails` for failed binding or request annotations. The prior fractional-input 500 came from the action dereferencing a null DTO after failed integer binding. Zero/negative quantities and negative prices previously reached the action despite ModelState errors. No custom parser or restored transition policy was added.

## Latest frontend and browser checks

These checks ran on 2026-10-26 before the subsequent documentation/MVC-validation pass. Frontend source is unchanged by that later pass.

- `pnpm.cmd --dir .\frontend build` passed. The initial production bundle was 934.61 kB, exceeding the configured 500 kB warning budget but below its 1 MB error budget.
- A temporary isolated Edge/Playwright check passed seven checks using separate API/frontend ports, the real proxy and no mocked API responses.
- It verified two seeded customers and form defaults; no submit for invalid form; numeric/currency/customer payloads and success feedback; repeat-reference feedback with no duplicate; newest-first listing, selected customer and server totals; list/detail-modal/close/status-update flows; and no browser page errors.
- Four temporary checks confirmed `oneOf` accepts allowed values, rejects others and retains strict value matching.

Temporary browser/server processes were stopped. The user's existing servers on 4200 and 5080 were left running with their in-memory data. At the end of that create/customer pass, the old 5080 process still returned 404 for the new customer route; a running process must load the new assembly before that endpoint is available. Current live-server state has not been rechecked by this documentation pass.

## Earlier snapshot and interpretation

Before the status checks were commented out, the create/customer pass ran 27 backend cases: 21 passed and 6 failed. After their removal, a fresh pre-repair run executed 27: 18 passed and 9 failed. Four of those later failures encoded the old Pending/terminal policy; the others exposed numeric-input and undefined-enum handling. These historical counts are not the final result above.

The source-level review also found the status action uses the body ID instead of its route ID, and the standalone detail route does not bind its ID to the component input. These are recorded findings, not fixes performed during the validation work.

No maintained frontend unit-test target is configured. A fresh locked install, current dependency audit, production deployment, final video and GitHub access/submission were not verified in this pass. No original brief file was found in the repository; the requirement review used the supplied excerpts.

## Scope and provenance

Documentation and the explicitly requested standard MVC validation/test adjustment were the only changes in this pass. Status restrictions stayed commented out; `OrderDb` and frontend submission logic were not refactored. No packages were added, no existing server was restarted, and no commit/push/publication/submission was performed.

Temporary TRX/browser artifacts are outside the repository under the task workspace; they are supporting execution evidence, not portable repository tests. Re-run the commands above to reproduce current results.
