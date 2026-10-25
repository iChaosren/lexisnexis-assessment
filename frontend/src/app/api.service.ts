import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChangeOrderStatusRequest } from './dtos/change-order-status-request';
import { CreateOrderRequest } from './dtos/create-order-request';
import { OrderResponse } from './dtos/order-response';
import { OrderSummaryResponse } from './dtos/order-summary-response';

@Injectable({ providedIn: 'root' })
export class ApiService {
  health(): Observable<string> {
    throw new Error('Not implemented.');
  }

  // Assessment excerpt: “Provides a simple way to submit a new order.”
  submitOrder(request: CreateOrderRequest): Observable<OrderResponse> {
    throw new Error('Not implemented.');
  }

  // Assessment excerpt: “Displays a list of orders showing the most recent first.”
  listOrders(): Observable<OrderSummaryResponse[]> {
    throw new Error('Not implemented.');
  }

  // Assessment excerpt: “Shows enough detail for a user to verify that totals and status look correct.”
  getOrder(id: string): Observable<OrderResponse> {
    throw new Error('Not implemented.');
  }

  // Assessment excerpt: “Optional if time permits: allow changing an order’s status from the UI or add simple client-side filtering.”
  changeOrderStatus(id: string, request: ChangeOrderStatusRequest): Observable<OrderResponse> {
    throw new Error('Not implemented.');
  }
}
