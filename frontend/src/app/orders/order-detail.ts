import {Component, inject, input, OnChanges, output, signal, SimpleChanges} from '@angular/core';
import {ApiService} from '../api.service';
import {OrderResponse} from '../dtos/order-response';

// Assessment excerpt: “Shows enough detail for a user to verify that totals and status look correct.”
@Component({
  selector: 'order-detail',
  templateUrl: './order-detail.html'
})
export class OrderDetail implements OnChanges {
  private readonly api = inject(ApiService);

  id = input<string>('');

  order = signal<OrderResponse | null>(null);
  loading = signal(false);
  error = signal(false);

  onClose = output<void>();

  ngOnChanges(changes: SimpleChanges<OrderDetail>): void {
    if (!changes.id?.currentValue) {
      this.order.set(null);
      this.loading.set(false);
      this.error.set(false);
      this.onClose.emit();
      return;
    }

    if (changes.id?.previousValue !== changes.id?.currentValue) {
      this.refetch();
    }
  }

  refetch() {
    this.api.getOrder(this.id()).subscribe({
      next: (order) => {
        this.order.set(order);
        this.loading.set(false);
        this.error.set(false);
      },
      error: (e) => {
        this.error.set(true);
        this.loading.set(false);
      }
    })
  }
}
