# Reading the current Angular app

Start with `src/app/orders/order-create.ts` and its adjacent `.html` file. The class owns form state and handlers; the template connects them to inputs. Separate templates are a readability choice. Angular also supports an inline `template` in the component decorator.

## File map

| File | Current role |
| --- | --- |
| `src/app/app.config.ts` | Configures HttpClient and the router through providers. |
| `src/app/app.routes.ts` | Defines the order routes and lazy component loading. |
| `src/app/app.ts` / `app.html` | Navigation and the router outlet. |
| `src/app/api.service.ts` | Shared HTTP methods for customers and orders. |
| `src/app/orders/order-create.ts` / `.html` | Customer options, component-local reactive form, and submit feedback. |
| `src/app/one-of.validator.ts` | Exported allowed-value validator used by the currency control. |
| `src/app/dtos/` | Request/response interfaces and currency/status enums. These types do not validate JSON at runtime. |
| `src/app/orders/order-list.ts` / `.html` | Order list signals, status dropdown handlers, and selected detail ID. |
| `src/app/orders/order-detail.ts` / `.html` | Detail input, HTTP read, modal, close event, and currency display. |

## Imports, providers, and injection

| Mechanism | What it does here |
| --- | --- |
| TypeScript `import {ApiService} ...` | Makes the class name available in the file. It does not obtain an instance. |
| `@Injectable({providedIn: 'root'})` | Registers ApiService with the root injector, so components share the service. |
| `provideHttpClient()` / `provideRouter(routes)` | Configure HTTP and routing in `app.config.ts`. |
| `inject(ApiService)` | Requests the service instance from Angular's injector. ApiService itself requests HttpClient with `inject(HttpClient)`. |
| Component `imports: [ReactiveFormsModule]` | Makes form directives available to the create template. It does not store form values. The root component similarly imports RouterLink and RouterOutlet for its template. |

`inject()` is a dependency lookup, not a wrapper. Field initializers run while Angular constructs a component or service, inside an injection context. Later handlers use the stored `this.api`; they do not call `inject()` again. There is no need to construct ApiService with `new` or provide another instance in each component.

See [Angular dependency injection](https://angular.dev/guide/di) and [HttpClient setup](https://angular.dev/guide/http/setup).

## Component-local FormBuilder and bindings

`OrderCreate` injects FormBuilder, then constructs its own `form` using `formBuilder.nonNullable.group(...)`. Sharing the builder service does not share that FormGroup. Each create component instance owns its controls; there is no central form service.

The current group contains `externalReference`, `customerId`, `currency`, and a `lines` FormArray. The array starts with one group containing `sku`, `name`, `quantity`, and `unitPrice`. `nonNullable` affects control typing and reset defaults; it does not add business validation.

The template connects those nested objects to HTML:

```html
<form [formGroup]="form" (ngSubmit)="submit()">
  <input formControlName="externalReference">
  <select formControlName="customerId">...</select>
  <fieldset formArrayName="lines">
    @for (line of form.controls.lines.controls; track line; let i = $index) {
      <div [formGroupName]="i">
        <input formControlName="sku">
      </div>
    }
  </fieldset>
</form>
```

`formArrayName` selects the array; `[formGroupName]="i"` selects one line group; each `formControlName` selects a control inside its current group. The actual template also binds the currency and remaining line inputs. HTML `name` attributes alone do not make those reactive-form connections.

`ngOnInit()` subscribes to `api.listCustomers()` and stores the returned options in `customers`. The template reads `customers()`, displays each name, and writes the selected customer's ID into `customerId` through `[value]="customer.id"`.

The current controls use `required`, numeric minimums, and `oneOf(Object.values(Currency))`. The extracted `oneOf` returns `null` when `allowed.includes(control.value)` is true, otherwise `{oneOf: true}`. Its behavior is unchanged by moving it to its own file.

The template currently displays one `submissionMessage()` rather than individual field error messages. Invalid submission marks the controls touched and displays the summary message. Frontend validators do not replace server validation; neither TypeScript types nor an HTML number input establish the server's error behavior.

See [reactive forms](https://angular.dev/guide/forms/reactive-forms).

## Submission and HTTP

`(ngSubmit)="submit()"` calls the component method. It guards against a second in-flight submission, checks `form.invalid`, and obtains a typed `CreateOrderRequest` with `getRawValue()`. This includes all control values, including any controls later disabled through the form API. The submit button is disabled while the request is in flight.

ApiService methods return HttpClient Observables. The component's `subscribe(...)` starts the request and handles its response or error. For submission, the current UI interprets a nonzero numeric response as submitted and zero as an existing-reference result. It then updates `submissionMessage` and clears `submitting`. It does not navigate away or reset the form.

The relevant methods are `listCustomers()` (`GET /api/customers`), `submitOrder()` (`POST /api/orders`), `listOrders()`, `getOrder(id)`, and `changeOrderStatus(id, request)`. Relative `/api` URLs use the development proxy configured through `angular.json` and `src/proxy.conf.json`. The service stores no form or order state.

## Signals and the existing detail modal

Read a signal with `orders()` or `selectedId()`; update it with `.set(...)`. The list holds orders and loading/error flags in signals. Its status select calls `updateStatus(...)`, which subscribes to the API request and logs zero-change results or errors. The dropdown's enum values describe the application's current options, not an assessment-mandated transition policy.

The list opens detail through `<order-detail [id]="selectedId()" ...>`. The detail component uses `input<string>('')`; `ngOnChanges` reacts to that input and fetches the selected order. Its `onClose` output tells the list to clear `selectedId`. This input/output modal flow is distinct from reading a route parameter: the current detail class does not use ActivatedRoute.

Detail stores the response and flags in signals. `currencyPrefix` is a `computed` value derived from the returned order's currency. The template displays returned line totals and formats subtotal, tax, and total for display. No effect or separate shared form state is used by these components.
