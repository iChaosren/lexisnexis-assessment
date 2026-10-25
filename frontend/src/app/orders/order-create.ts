import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { apiErrorMessage } from '../api-error';
import { ApiService } from '../api.service';

// Assessment excerpt: “Provides a simple way to submit a new order.”
@Component({
  selector: 'app-order-create',
  // Makes [formGroup], formControlName, and formArrayName usable in this template.
  imports: [ReactiveFormsModule],
  templateUrl: './order-create.html'
})
export class OrderCreate {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  // FormBuilder is injectable; each component instance owns its own form state.
  // No central form service or root-level form object is required.
  protected readonly form = this.fb.nonNullable.group({
    clientReference: ['', Validators.required],
    lines: this.fb.array([this.createLine()])
  });
  protected readonly message = signal('Submitting calls an unimplemented endpoint (HTTP 501).');
  protected readonly submitting = signal(false);

  private createLine() {
    // Assessment excerpt: “Quantities are positive whole numbers; prices are non-negative.”
    // These Validators are frontend UX examples only. Backend validation is still TODO.
    // Editable numeric input defaults are convenience examples, not server decisions.
    return this.fb.nonNullable.group({
      description: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1), Validators.pattern(/^[1-9]\d*$/)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  protected addLine(): void {
    this.form.controls.lines.push(this.createLine());
  }

  protected submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.message.set('Check the field messages. These checks are a frontend learning example.');
      return;
    }

    this.submitting.set(true);
    this.message.set('Calling the submit stub...');
    // Assessment excerpt: “Line totals and overall totals are calculated on the server.”
    // Send inputs only. Do not calculate or send totals from this form.
    this.api.submitOrder(this.form.getRawValue()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.message.set('The API returned a response. TODO: design the completed submission flow.');
        this.submitting.set(false);
      },
      error: (error: unknown) => {
        this.message.set(apiErrorMessage(error));
        this.submitting.set(false);
      }
    });
  }
}
