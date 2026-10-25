# Reading this Angular scaffold

Start with `src/app/orders/order-create.ts` and its adjacent `.html` file. The class owns state and handlers; the template binds them to the page. Separate templates are a readability choice here. An inline `template: '...'` is also supported.

## Three different kinds of import/registration

| Mechanism | Where it appears here | What it does |
| --- | --- | --- |
| TypeScript `import { ApiService } ...` | At the top of a `.ts` file | Makes the class/type name available to this source file. It does not obtain a service instance. |
| DI provider | `@Injectable({ providedIn: 'root' })` on `ApiService`; `provideHttpClient()` and `provideRouter(routes)` in `app.config.ts` | Tells Angular how to supply/configure dependencies. The root ApiService is shared across this application. |
| `inject(ApiService)` | A component field initializer | Requests the instance from Angular's current injector. It can resolve ApiService's own HttpClient dependency too. |
| Component `imports: [ReactiveFormsModule]` | The create/detail components | Makes directives such as `[formGroup]` and `formControlName` available to that component's template. It does not hold the form's values. |

`inject()` is a dependency lookup, not a wrapper around the service. Angular creates components inside an injection context, so a field such as `private readonly api = inject(ApiService)` works while constructing the component. Call the stored `this.api` later in handlers; do not call `inject()` from an arbitrary click handler. Constructor injection is another valid style, for example `constructor(private api: ApiService) {}`. This scaffold consistently uses field injection.

You do not manually construct `new ApiService(...)` or list ApiService again in every component's `providers`. A component-level provider would deliberately create a more local instance; that is unnecessary for this small HTTP client.

See [Angular dependency injection](https://angular.dev/guide/di) and [setting up HttpClient](https://angular.dev/guide/http/setup).

## FormBuilder and per-field errors

`OrderCreate` requests Angular's FormBuilder service in a class field:

```typescript
private readonly fb = inject(FormBuilder);
protected readonly form = this.fb.nonNullable.group({
  clientReference: ['', Validators.required],
  lines: this.fb.array([this.createLine()])
});
```

FormBuilder is a helper that constructs controls. **The new FormGroup and its controls belong to this component instance.** Sharing the builder service does not share the form. Navigating to a new create component creates new form state. There is no requirement for a central form service; add shared form state only if a later feature actually needs it.

The component adds `ReactiveFormsModule` to its template imports. The template then connects the instance to the DOM:

```html
<form [formGroup]="form" (ngSubmit)="submit()" novalidate>
  <input formControlName="clientReference" />
</form>
```

`Validators.required` produces a `required` error on that control. `Validators.min(...)` produces a `min` error, and `Validators.pattern(...)` produces a `pattern` error. These are Angular control properties, not error messages sent from a central service. The template chooses the wording and when to show it:

```html
@if (form.controls.clientReference.touched || form.controls.clientReference.dirty) {
  @if (form.controls.clientReference.hasError('required')) {
    <p>Enter a client reference.</p>
  }
}
```

`touched` means the user has left the field; `dirty` means its value was changed through the UI. `submit()` calls `markAllAsTouched()` so an empty submission reveals the messages. It checks `form.invalid` before sending. `getRawValue()` gives the typed request input, including controls that may later be disabled. The FormArray groups each input line and lets the example add another local row.

Try `/orders/new`: submit empty fields, enter quantity `1.5`, or price `-1`, then fix the values. These validators are **frontend UX learning examples**. The backend still needs its own validation, duplicate handling, calculations, and error design; a client can bypass the UI. Required reference/description fields are provisional scaffold choices rather than a verbatim statement of all assessment rules. No line or overall totals are calculated here.

See [reactive forms](https://angular.dev/guide/forms/reactive-forms) and [form validation](https://angular.dev/guide/forms/form-validation).

## ApiService and HTTP

`src/app/api.service.ts` is a small injectable class. It stores no orders or forms. Each method describes an HTTP request and returns an Observable. Components call a method and subscribe to send the request; a TypeScript response type describes the intended shape but does not validate returned JSON at runtime.

The chain is:

```text
app.config.ts configures HTTP/router
  -> Angular constructs a routed component
  -> component injects ApiService
  -> ApiService injects HttpClient
  -> component subscribes to a method's Observable
  -> /api request goes through the development proxy
  -> backend stub returns HTTP 501 ProblemDetails
  -> component updates its message signal
  -> template reads that signal
```

All four order methods are real request wiring into explicit unimplemented endpoints. `api-error.ts` labels HTTP 501 as a TODO scaffold. No fake successful save, list, detail, or status change is provided. Status is free text because the allowed vocabulary and transitions are still your design work. The placeholder detail link uses an example GUID only; it does not represent a saved order.

`takeUntilDestroyed(this.destroyRef)` cleans up the component subscription when that component is destroyed. The detail page uses `switchMap` so a changed route ID cancels its prior read, and handles errors inside that read so a 501 does not stop later route changes.

## Signals and lifecycle

The message/loading signals are small pieces of view state. Read a signal with `message()` and update it with `message.set(...)`. Angular tracks template reads and refreshes the view. The list/detail components start their initial reads in `ngOnInit`; user actions use ordinary methods. No effect is needed for these examples.

The reactive form model is already its own state system. It does not need to be copied into a signal or a global service just to show field errors.

## Where to work next

The route map is in `src/app/app.routes.ts`. The backend extension points are the four methods in `../backend/Assessment.Api/OrdersEndpoints.cs` when viewed from the repository root's `frontend` folder. Review the provisional request/response types on both sides before implementing. Keep `SOLUTION.md` for your own eventual decisions, and extend `AI_ASSISTANCE.md` when using further help.
