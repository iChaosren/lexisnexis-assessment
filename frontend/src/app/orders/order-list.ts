import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { apiErrorMessage } from '../api-error';
import { ApiService } from '../api.service';
import { OrderSummaryResponse } from '../dtos/order-summary-response';

// Assessment excerpt: “Displays a list of orders showing the most recent first.”
@Component({
  selector: 'app-order-list',
  imports: [RouterLink],
  templateUrl: './order-list.html'
})
export class OrderList implements OnInit {
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly orders = signal<OrderSummaryResponse[] | null>(null);
  protected readonly message = signal('');
  protected readonly loading = signal(false);

  ngOnInit(): void {
    this.loadOrders();
  }

  protected loadOrders(): void {
    this.loading.set(true);
    this.message.set('Loading from the API scaffold...');
    this.api.listOrders().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (orders) => {
        // TODO: implement newest-first ordering in the backend; do not fake data here.
        this.orders.set(orders);
        this.message.set('Response received from the API.');
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.orders.set(null);
        this.message.set(apiErrorMessage(error));
        this.loading.set(false);
      }
    });
  }
}
