import { CreateOrderLineRequest } from './create-order-line-request';

export interface OrderLineResponse extends CreateOrderLineRequest {
  lineTotal: number;
}
