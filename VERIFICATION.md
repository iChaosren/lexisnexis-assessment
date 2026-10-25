# Scaffold verification

Latest expansion checks on Clayton-PC, 2026-10-25. Initial scaffold history and its previous verification record remain in commit `b58bb19`.

## Scope and versions

The scaffold uses .NET SDK 10.0.401, target net10.0, ASP.NET Core runtime 10.0.12, Node 22.20.0, installed pnpm 12.14.0, Angular/framework/forms/router 21.2.25, Angular CLI/build 21.2.28, TypeScript 5.9.3, RxJS 7.8.2, and patched transitive Undici 7.30.1. Backend test dependencies remain Microsoft.AspNetCore.Mvc.Testing 10.0.12, Microsoft.NET.Test.Sdk 17.14.1, xUnit 2.9.3 and Visual Studio runner 3.1.4.

The only current solution is `backend/LexisNexis.Assessment.sln`, containing Assessment.Api and Assessment.Api.Tests. Original `.slnx` and npm lockfile remain recoverable in Git history; they are no longer competing active files.

## Preserving the working folder

Before changes, the final repository was inspected and matched `b58bb19` with no tracked changes. Existing ignored `.idea` content was left alone. Changes were prepared in an isolated local clone at `C:\Users\myccb\Documents\Codex\2026-10-25\task\lexisnexis-expansion`. Touched files are checked against the original Git blob before applying, and new paths must be absent. This preserves unrelated/concurrent user work rather than copying an entire tree over it.

## Completed isolated checks

| Check | Result |
| --- | --- |
| `pnpm.cmd --dir .\frontend import` | Imported original npm lock into pnpm 12 format. |
| `pnpm.cmd --dir .\frontend install --frozen-lockfile` | Passed; matching forms/router packages and lockfile. |
| `pnpm.cmd --dir .\frontend audit` | No known vulnerabilities reported. |
| `pnpm.cmd --dir .\frontend run build` | Passed; initial production bundle 251.56 kB plus lazy route chunks. |
| `pnpm.cmd --dir .\frontend list --depth=0` | Twelve direct packages matched expected versions. |
| `dotnet restore .\backend\LexisNexis.Assessment.sln --locked-mode` | Passed for both projects. |
| `dotnet build .\backend\LexisNexis.Assessment.sln --no-restore --configuration Release` | Passed; zero warnings and errors. |
| `dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release --list-tests` | Discovered health case plus four endpoint theory cases. |
| `dotnet test .\backend\LexisNexis.Assessment.sln --no-build --configuration Release` | Five passed, zero failed, zero skipped. |

Five backend cases cover real pipeline/route wiring only: health 200 and four operation-specific 501 ProblemDetails. No business tests have been implemented or counted as coverage. No empty or skipped business tests were added.

pnpm initially finished downloading but returned `ERR_PNPM_IGNORED_BUILDS` for named native Angular dependencies. Their install scripts were inspected and added to the project-local allowBuilds map. The subsequent frozen install passed. There was no repeated retry of the unresolved condition and no global package-manager/configuration change. Build/restore/test required the normal approved execution environment because the filesystem/network sandbox did not provide usable execution.

## Actual server and browser smoke checks

At **2026-10-25T19:05:55Z**, both documented commands were started from the isolated copy: `dotnet run --project .\backend\Assessment.Api` and `pnpm.cmd --dir .\frontend start`. Ports were free before launch. Direct API health and proxied health both returned HTTP 200 with `Healthy`; Angular root returned 200.

A temporary Playwright-core 1.56.1 script outside the repository used installed Edge headlessly with an isolated temporary context, no persistent user profile, and no API interception/mocks. It passed ten real-browser checks:

- List route mounts and shows the real 501 response, with no fake order rows.
- New-order navigation mounts its component and empty submit shows reference/description field messages without sending a request.
- Quantity 1.5 and price -1 show their individual UX messages.
- A valid example input sends only reference/line inputs, no totals, and receives 501.
- Add-line adds local FormArray controls without creating an order.
- Placeholder detail route requests the example GUID and shows 501, with no fake detail.
- Blank optional status input shows its field message without sending PATCH.
- A free-text status input sends the expected PATCH body and receives 501.
- The health button reaches the API through the frontend proxy and displays Healthy.
- There are no unexpected page/console errors; expected 501 resource errors are excluded.

Both server/listener process trees and the temporary browser were closed. Cleanup confirmed no listeners on 4200 or 5080. Temporary smoke tooling is task-local and is not a maintained frontend test suite or a new project dependency.

## Intentional TODOs and limits

No persistence, order creation, duplicate/reference semantics, newest-first sorting, server validation, totals/rounding, status vocabulary or transition rules have been implemented. DTOs, route names, GUID shape and UI required-field examples are provisional scaffold choices. Ordinary ASP.NET JSON/route binding still exists. Response DTOs describe future shapes and do not provide fake responses.

Frontend validators are learning/UX examples, not backend business validation. The optional status UI is only request wiring. SOLUTION.md remains unfilled, and no candidate reasoning or final video has been fabricated. No remote creation, publication, sharing, submission, recruiter contact or REA server/tool use occurred.

## Final repository recheck

The guarded patch was applied successfully to **C:\Github\lexisnexis-assessment** after verifying all 30 touched paths against the original and confirming new paths did not exist. The ignored IDE folder was preserved.

At the final path, the exact README pnpm install command passed with the normal pnpm store (no task-cache flag needed), the production build passed with the same 251.56 kB initial bundle, and audit reported no known vulnerabilities. Conventional-solution locked restore/build, test discovery, and test execution were repeated successfully: zero build warnings/errors; five passed, zero failed/skipped. `dotnet sln ... list` confirmed both API/test projects.

The full ten-check real-browser run was repeated at **2026-10-25T19:10:13Z** against this final path. All routes, field errors, real 501 request bodies/responses, and health proxy passed again with no unexpected browser errors. Both temporary servers and the isolated browser were stopped; no listener remained on 4200 or 5080.

Staged files were checked against the isolated patch, and touched working files were checked for concurrent edits before committing. `git diff --cached --check` passed. No unrelated user files were staged for this change.

At handoff, `git remote -v` showed an existing origin of `https://github.com/iChaosren/lexisnexis-assessment.git`. This task did not configure that remote or create a GitHub repository, and did not fetch from or push to it. The configuration was preserved. The statement above about remote creation/publication describes this assistant's actions, not a claim about the user's repository history.
