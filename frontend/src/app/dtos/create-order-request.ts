import { CreateOrderLineRequest } from './create-order-line-request';

// Assessment excerpt: “You design the API, data model, validation, and error behavior.”
export interface CreateOrderRequest {
  externalReference: string;
  currency: number;
  lines: CreateOrderLineRequest[];
}
