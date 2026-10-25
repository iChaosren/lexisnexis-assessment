import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ChangeOrderStatusRequest, CreateOrderRequest, OrderResponse, OrderSummaryResponse } from './order-contracts';

// Registers one shared service at the root injector. It does not hold form state.
@Injectable({ providedIn: 'root' })
export class ApiService {
  // inject() requests the configured instance; a TypeScript import alone does not do DI.
  private readonly http = inject(HttpClient);
  private readonly ordersUrl = '/api/orders';

  health(): Observable<string> {
    return this.http.get('/api/health', { responseType: 'text' });
  }

  // Assessment excerpt: “Provides a simple way to submit a new order.”
  // TODO: the API currently returns 501. Subscribing sends the request; no order is saved.
  submitOrder(request: CreateOrderRequest): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(this.ordersUrl, request);
  }

  // Assessment excerpt: “Displays a list of orders showing the most recent first.”
  // TODO: the server must supply the required ordering; no client sorting is implemented.
  listOrders(): Observable<OrderSummaryResponse[]> {
    return this.http.get<OrderSummaryResponse[]>(this.ordersUrl);
  }

  // Assessment excerpt: “Shows enough detail for a user to verify that totals and status look correct.”
  getOrder(id: string): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`${this.ordersUrl}/${encodeURIComponent(id)}`);
  }

  // Assessment excerpt: “Optional if time permits: allow changing an order’s status from the UI or add simple client-side filtering.”
  changeOrderStatus(id: string, request: ChangeOrderStatusRequest): Observable<OrderResponse> {
    return this.http.patch<OrderResponse>(`${this.ordersUrl}/${encodeURIComponent(id)}/status`, request);
  }
}
