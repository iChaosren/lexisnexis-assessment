import {Component, inject, OnInit, signal} from '@angular/core';
import {ApiService} from '../api.service';
import {OrderResponse} from '../dtos/order-response';
import {OrderDetail} from './order-detail';
import {OrderStatus} from '../dtos/order-status';

// Assessment excerpt: “Displays a list of orders showing the most recent first.”
@Component({
  selector: 'order-list',
  imports: [
    OrderDetail
  ],
  templateUrl: './order-list.html'
})
export class OrderList implements OnInit {
  private readonly api = inject(ApiService);

  selectedId = signal('');
  orders = signal<OrderResponse[]>([]);
  loading = signal(true);
  error = signal(false);

  statusOptions: OrderStatus[] = Object.values(OrderStatus);

  ngOnInit(): void {
    this.api.listOrders().subscribe({
      next: (orders) => {
        this.loading.set(false);
        this.error.set(false);
        this.orders.set(orders)
      },
      error: (error) => {
        this.loading.set(false);
        this.error.set(true);
        console.error(error);
      }
    })
  }

  updateStatus(orderId: string, event: Event) {

  }
}
