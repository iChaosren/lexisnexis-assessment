// Assessment excerpt: “You design the API, data model, validation, and error behavior.”
// These names/types are provisional scaffold choices, mirrored in OrderContracts.cs.
// They are transport shapes, not a completed domain model or validation specification.
export interface CreateOrderLineRequest {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface CreateOrderRequest {
  clientReference: string;
  lines: CreateOrderLineRequest[];
}

export interface ChangeOrderStatusRequest {
  status: string; // TODO: Clayton chooses statuses and permitted transitions on the server.
}

export interface OrderSummaryResponse {
  id: string;
  clientReference: string;
  createdAtUtc: string; // JSON timestamp; JavaScript does not automatically turn this into Date.
  status: string;
  total: number;
}

export interface OrderLineResponse extends CreateOrderLineRequest {
  lineTotal: number;
}

export interface OrderResponse extends OrderSummaryResponse {
  lines: OrderLineResponse[];
}
