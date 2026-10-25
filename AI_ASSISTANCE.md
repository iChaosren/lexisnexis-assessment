# AI assistance log

## 2026-10-25 — initial scaffold

**Contributor:** OpenAI Codex. This log was written by the assistant. It does not claim that Clayton has reviewed the scaffold or adopted the assistant's choices.

### User prompt (verbatim)

> Can you make a boilerplate repo and project for me, for when I start work on the assessment?
> I don’t even know how to setup angular, let alone how it works…

### Execution brief supplied to the assistant (summary, not a verbatim user quote)

Create a minimal runnable local Git repository for Clayton's LexisNexis assessment on Clayton-PC, preferably in a new folder under C:\Github after checking for existing work. Setup only: Angular standalone, ASP.NET Core on installed stable .NET 8+ tooling, a health-only API, a meaningful infrastructure smoke test, local proxy wiring, and documentation. Verify official compatibility, builds, test discovery/execution, and both servers. Include exact Windows commands, a React-to-Angular file map, an unfilled SOLUTION.md, and an honest AI prompt log. Do not implement order domain, business rules, CRUD, validation, deduplication, totals, or status flows. Do not publish, submit, share, contact recruiters, create credentials, or run REA tools/server processes. Commit locally only if an existing Git identity is available.

### Assistant delegation prompts

The assistant used a read-only helper for two tasks:

1. Verify current official Angular/Node/TypeScript compatibility and recommend stable versions using the installed tools; suggest one meaningful ASP.NET health smoke test with minimal dependencies. No file edits or server processes.
2. Review the staged scaffold for setup-only scope, backend test correctness, frontend health/proxy wiring, and leftover generated test or MCP configuration. No edits or business implementation.

These are summaries of internal helper prompts, not statements from Clayton. The helper reported compatible Angular 21 tooling and no blocking code issues in its read-only scaffold review.

### Work produced by AI

- Generated the Angular standalone workspace and .NET API/test solution using official CLI templates, then reduced the starter to a health-check page and endpoint.
- Selected .NET 10 because SDK 10.0.401 was installed and is LTS. Selected Angular 21 with TypeScript 5.9 because the installed Node 22.20.0 is supported by that combination; Angular 22 requires a newer Node release and TypeScript 6.
- Added HttpClient/signal health wiring, the local development proxy, and one test of the real ASP.NET health endpoint. This is infrastructure integration coverage, not assessment business coverage.
- Added Git ignore rules, pinned dependencies and lockfiles, run/build/test instructions, the file map, this log, and the unfilled decision template.
- Used a temporary npm 11.6.2 invocation to generate the initial lockfile after installed npm 10.9.3 crashed in optional-peer dependency resolution. A subsequent clean install with installed npm 10.9.3 succeeded; no global package manager was changed.
- Inspected npm audit's reported Angular build dependency advisory and pinned the same-major patched transitive `undici` 7.30.1 via a package override. Rechecked the locked install, audit, build, and startup after that change.
- Ran the checks recorded in VERIFICATION.md. All assessment implementation and business tests remain Clayton's work.

These are assistant setup decisions. No personal implementation reasoning from Clayton has been fabricated. Continue this file with later prompts and, separately, Clayton's actual review and reasoning.

## 2026-10-25 — endpoint and Angular learning stubs; pnpm migration

### Follow-up user prompt (verbatim excerpts)

> Not seeing a solution file for the backend there gizmo, also this backend isn't exactly setup for easy addition. Please add me all the boilerplate for the endpoints I'll need.
> And same for the frontend, just stubs and boilerplate I can add things to and learn as I go.
> Include comments with the exact excerpt from the assessment for what it's intended to do to the boilerplate, so I have a better idea of why chose to add certain bits.

> Not sure why you went with `npm` as the package manager, I have `yarn` and `pnpm` installed, personally prefer that.

> For the forms, I'm thinking to inject FormBuilder into the project and using Validators for frontend validation. Not sure where to do that in Angular, or whether it allows for per field validation errors/messages.

> HTTP seems quite straightforward as well, just need to write an ApiService and inject it as well, once again, not sure where or how to inject that beside just `inject(ApiService)`...

These are excerpts, not a reproduction of the entire follow-up message. The execution brief authorized stubs and learning validation examples, while keeping backend business implementation out of scope. It supplied the exact assessment excerpts used in source comments.

### Assistant helper prompts (summaries)

- Backend helper: add one conventional solution in backend, provisional contracts and four endpoint extension points returning explicit 501; quote the supplied assessment excerpts; add wiring tests while leaving business rules TODO. Edit only the isolated clone's backend and retired root solution; no final-target edits or commit.
- Frontend helper: add standalone routes/pages, root ApiService, local FormBuilder examples and per-field errors; call real 501 stubs and avoid fake data, totals, sorting or status policy. Edit only frontend source in the isolated clone.
- Frontend helper follow-up: write a temporary browser smoke script outside the repository to verify actual routes, form messages, requests and expected 501 responses using installed Edge; no server or dependency installation by the helper.

### AI choices and work

- Inspected the local Git state before work; it matched `b58bb19`. Preserved the existing ignored IDE folder. Prepared changes in an isolated local clone and checked touched paths against the original before applying, to avoid overwriting concurrent user edits.
- Replaced the root `.slnx` with `backend/LexisNexis.Assessment.sln`, keeping one current solution. The original remains in Git history.
- Chose provisional HTTP routes, DTO field names, GUID IDs, timestamp shape, response-only total fields and free-text status. These are scaffolding choices, not assertions that the brief mandates them or that Clayton has adopted them. No status vocabulary/transition or monetary calculation was chosen.
- Added endpoint signatures and operation-specific 501 ProblemDetails. Added wiring tests rather than business behavior or business coverage claims.
- Added Angular list/create/detail components, separate templates, ApiService methods, and component-local forms. Frontend required/min/pattern validation and per-field wording are learning/UX examples only. Required reference/description controls are provisional choices. Backend validation remains TODO.
- Used installed pnpm 12.14.0, imported the existing lockfile, added forms/router at matching Angular versions, and moved the patched Undici override to pnpm's local settings. Named native build dependencies are explicitly allowed; no global install/configuration was changed. The old npm lock is preserved by Git history rather than maintained alongside pnpm's lock.
- Updated README and added frontend/LEARNING.md to distinguish providers, injection and template imports; explain local forms, root HTTP service, subscriptions and signals; and identify remaining candidate decisions.
- Ran the checks recorded in VERIFICATION.md. No assessment business implementation, public remote, sharing, submission or recruiter contact was performed.

Clayton's reasoning and endorsement have not been fabricated. SOLUTION.md remains his unfilled decision template.
