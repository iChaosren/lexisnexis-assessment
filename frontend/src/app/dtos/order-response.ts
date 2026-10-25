import { CustomerResponse } from './customer-response';
import { OrderLineResponse } from './order-line-response';

export interface OrderResponse {
  id: string;
  externalReference: string;
  customerId: string;
  customer: CustomerResponse;
  createdAt: string;
  updatedAt: string;
  status: number;
  currency: number;
  lines: OrderLineResponse[];
  subTotal: number;
  tax: number;
  total: number;
}
