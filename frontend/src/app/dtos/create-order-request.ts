import { CreateOrderLineRequest } from './create-order-line-request';
import {Currency} from './currency';

// Assessment excerpt: “You design the API, data model, validation, and error behavior.”
export interface CreateOrderRequest {
  externalReference: string;
  currency: Currency;
  lines: CreateOrderLineRequest[];
}
