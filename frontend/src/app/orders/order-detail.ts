import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EMPTY, catchError, switchMap } from 'rxjs';
import { apiErrorMessage } from '../api-error';
import { ApiService } from '../api.service';
import { OrderResponse } from '../dtos/order-response';

// Assessment excerpt: “Shows enough detail for a user to verify that totals and status look correct.”
@Component({
  selector: 'app-order-detail',
  imports: [ReactiveFormsModule],
  templateUrl: './order-detail.html'
})
export class OrderDetail implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly id = signal('');
  protected readonly order = signal<OrderResponse | null>(null);
  protected readonly message = signal('');
  protected readonly statusMessage = signal('Status names and transition rules are still TODO on the server.');
  protected readonly changingStatus = signal(false);
  protected readonly statusForm = this.fb.nonNullable.group({ status: ['', Validators.required] });

  ngOnInit(): void {
    this.route.paramMap.pipe(
      // switchMap cancels the previous read if navigation changes the ID.
      switchMap((parameters) => {
        const id = parameters.get('id') ?? '';
        this.id.set(id);
        this.order.set(null);
        this.message.set('Calling the detail stub...');
        return this.api.getOrder(id).pipe(
          catchError((error: unknown) => {
            this.message.set(apiErrorMessage(error));
            return EMPTY; // Keep listening for later route changes after the expected 501.
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe((order) => {
      this.order.set(order);
      this.message.set('Response received from the API.');
    });
  }

  protected changeStatus(): void {
    // Assessment excerpt: “Optional if time permits: allow changing an order’s status from the UI or add simple client-side filtering.”
    // This is request wiring only: no enum, transition policy, or optimistic status change.
    this.statusForm.markAllAsTouched();
    if (this.statusForm.invalid) {
      this.statusMessage.set('Enter a status string to exercise the stub.');
      return;
    }

    this.changingStatus.set(true);
    this.statusMessage.set('Calling the change-status stub...');
    this.api.changeOrderStatus(this.id(), this.statusForm.getRawValue()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.statusMessage.set('The API returned a response. TODO: design completed status feedback and refresh behavior.');
        this.changingStatus.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(apiErrorMessage(error));
        this.changingStatus.set(false);
      }
    });
  }
}
