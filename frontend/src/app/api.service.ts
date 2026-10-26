import {inject, Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import { ChangeOrderStatusRequest } from './dtos/change-order-status-request';
import { CreateOrderRequest } from './dtos/create-order-request';
import { OrderResponse } from './dtos/order-response';
import {HttpClient} from '@angular/common/http';
import {CustomerResponse} from './dtos/customer-response';

@Injectable({ providedIn: 'root' })
export class ApiService {
  http = inject(HttpClient);

  health(): Observable<string> {
    return this.http.get<string>(`/api/health`)
  }

  listCustomers(): Observable<CustomerResponse[]> {
    return this.http.get<CustomerResponse[]>(`/api/customers`);
  }

  // Assessment excerpt: “Provides a simple way to submit a new order.”
  submitOrder(request: CreateOrderRequest): Observable<number> {
    return this.http.post<number>(`/api/orders`, request);
  }

  // Assessment excerpt: “Displays a list of orders showing the most recent first.”
  listOrders(): Observable<OrderResponse[]> {
    return this.http.get<OrderResponse[]>(`/api/orders`);
  }

  // Assessment excerpt: “Shows enough detail for a user to verify that totals and status look correct.”
  getOrder(id: string): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`/api/orders/${id}`);
  }

  // Assessment excerpt: “Optional if time permits: allow changing an order’s status from the UI or add simple client-side filtering.”
  changeOrderStatus(id: string, request: ChangeOrderStatusRequest): Observable<number> {
    return this.http.patch<number>(`/api/orders/${id}/status`, request);
  }
}
