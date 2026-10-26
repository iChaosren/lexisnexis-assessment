# AI assistance log

Written by OpenAI Codex and updated on 2026-10-26 from the available conversation, Git history and current files. Quoted prompts below are verbatim excerpts unless labelled otherwise; summaries are not a reconstructed transcript. This records substantial AI involvement and user-led work without claiming Clayton endorsed every assistant choice or inventing his reasoning.

## 2026-10-25: initial setup

User prompt (verbatim):

> Can you make a boilerplate repo and project for me, for when I start work on the assessment?
> I don’t even know how to setup angular, let alone how it works…

Execution brief (summary): create a small runnable local repository, Angular standalone frontend, ASP.NET Core backend and infrastructure test; setup only, with no assessment business implementation. Verify compatibility, commands, builds and server startup. Do not publish or submit.

The assistant generated the projects, selected the installed compatible .NET 10/Angular 21 toolchain, added health/proxy wiring, pinned dependencies and wrote the original documentation. It repaired an npm lock-generation problem with a temporary npm invocation and added a project-local Undici override after checking an advisory. The initial scaffold was committed locally. These were assistant setup choices, not Clayton's personal rationale.

## 2026-10-25: endpoint scaffolding, pnpm and overreach

User prompt excerpts:

> Please add me all the boilerplate for the endpoints I'll need.
> And same for the frontend, just stubs and boilerplate I can add things to and learn as I go.

> Not sure why you went with `npm` as the package manager, I have `yarn` and `pnpm` installed, personally prefer that.

The user also asked about FormBuilder, field validation, injection and HttpClient (summary). The assistant added a conventional `.sln`, provisional contracts, explicit 501 order handlers, Angular routes/pages, HTTP methods, forms, validation messages and page behavior. It migrated the local package setup to installed pnpm without global changes.

The frontend work exceeded the requested empty stubs/learning scope. Clayton objected and explicitly approved removing the assistant's frontend implementation. The assistant reduced its components/services to empty stubs, preserving DTOs, configuration, Git history, a scoped backup and current user changes. Early documentation saying all business work remained Clayton's was only true of the initial scaffold, not a claim about the later combined codebase.

## 2026-10-25: user model, controllers and tests

User instructions (summary): use his added entities and database abstraction; replace record contracts with separate DTO classes, add MVC controllers and singleton injection, fill mapping methods, seed small test data, and split frontend interfaces. Preserve his files and avoid unrelated implementation.

The assistant performed that migration, mappings and seeding. Clayton supplied and developed the entities, `IOrderDb`/`OrderDb`, reference-derived IDs and order behavior. He later implemented the frontend list, detail modal, status updates and local form/validator work. This is a scope summary supported by the conversation and changes, not a line-by-line authorship audit. The assistant also modified these areas when specifically requested, including the later customer association.

User test request (verbatim excerpt):

> Please add the necessary unit tests for the endpoints please. And I want the code looking pristine. Remember the commandments.

The assistant added direct controller tests and focused HTTP checks. Clayton then objected to awkward names and handwritten/string-oriented inputs. The assistant revised the tests to short behavior names, typed DTOs and typed JSON reads/writes. A fractional quantity uses an anonymous wire object because the request DTO's integer cannot hold `1.5`. Failures were reported rather than changing product logic to make tests pass. Clayton supplied the missing `MapControllers` fix; the assistant reran against it.

## 2026-10-25: styling, modal and enum JSON

User prompt excerpts:

> Okay cool. Can you add Bulma to the frontend so there's at least some basic styling and some html ONLY boilerplate for the components.

> Please make the html of order details a modal

> On the backend, please add JSON configuration to return enums as strings automatically.

The assistant installed Bulma locally with pnpm, imported its CSS and added static HTML templates. A later HTML-only pass wrapped the detail content in a Bulma modal, leaving open/close wiring to Clayton. It added `JsonStringEnumConverter()` to existing MVC JSON options, preserving enum name casing and default integer acceptance. It verified serialization/deserialization using a temporary isolated API instance and stopped it. Clayton subsequently wired the modal and other views himself.

## 2026-10-26: create form and existing customers

User prompt excerpt:

> Hook up the basics on the html side. Input <-> Form
> And Form Submit.
>
> You may need to add a basic Customer List api so you can select which customer an order is linked to as well.
>
> Just use the existing two Customers in the seeded data.

Additional user prompts:

> Also, do me a favour and extract the oneOf validator function into a separate .ts file please.

> You may also need to update the tests, given that I've altered the Ids to match how things are structured.

The assistant bound the existing FormGroup/FormArray to HTML, subscribed to submission, and added submission state plus success/duplicate/error feedback. It added customer listing from the existing seeded entities, a customer ID request field and selection/association. No new sample customers were added. It extracted `oneOf` without changing its behavior and aligned test fixtures with Clayton's current IDs and string enums.

Builds, tests, an isolated real-browser flow and small temporary validator checks were run. The browser verified creation, duplicates, customer/totals, list, detail and status updates. Existing user servers/data were preserved; temporary processes stopped. Full test results were not all passing; see [VERIFICATION.md](VERIFICATION.md).

## 2026-10-26: feedback, documentation and MVC validation

User feedback (verbatim excerpts):

> Hmmm... It did slightly more than I wanted on the form submission side of things, but it's looking mostly good.
> Failing the lick tests here in OrderDb...

> Commented out the Status checks. Just made more sense to me to have them, but I guess that wasn't in the spec.

> Update all the README files etc. and AI logs as needed.

The assistant recorded that feedback and did not refactor `OrderDb`, reduce submission behavior or restore the commented-out transition checks. Documentation was rewritten against the implementation, separating facts, recommendations and missing verification. No specific Pending/terminal transition policy is presented as an assessment requirement.

After an explanation of standard `[ApiController]` invalid-model handling, the user replied:

> Okay, please address that. Why isn't it doing normal Model State validation?

The assistant's earlier controller setup lacked `[ApiController]`. The request annotations were already on properties, but MVC did not automatically stop the action on invalid ModelState; fractional input left a null DTO and caused a 500. The assistant added the standard controller attribute, strengthened HTTP validation checks, and updated tests that still assumed the removed transition restrictions. It did not add custom parsing or restore those restrictions. Final outcomes and unresolved issues are in [VERIFICATION.md](VERIFICATION.md).

## Attribution and limits

The assistant used helper agents for bounded code/doc work, compatibility review, ownership checks and verification. This paragraph summarizes those internal tasks; it is not an exact delegation transcript. Temporary test/browser tooling lived outside the repository and is not a maintained frontend test suite.

This log is not a claim that Clayton wrote all current code or that AI wrote all current code. Clayton's own review, explanations and final video remain his work. The assistant did not create credentials, run REA tools, publish/push the later changes, submit the assessment, share it, or contact recruiters. Existing remote configuration and user-created commits were preserved.
