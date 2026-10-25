import { CustomerResponse } from './customer-response';
import { OrderLineResponse } from './order-line-response';
import {OrderStatus} from './order-status';
import {Currency} from './currency';

export interface OrderResponse {
  id: string;
  externalReference: string;
  customerId: string;
  customer: CustomerResponse;
  createdAt: string;
  updatedAt: string;
  status: OrderStatus;
  currency: Currency;
  lines: OrderLineResponse[];
  subTotal: number;
  tax: number;
  total: number;
}
